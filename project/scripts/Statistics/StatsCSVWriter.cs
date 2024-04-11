using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
// TODO: Write straight to excel and build charts automatically
// using Microsoft.Office.Interop.Excel.ChartEvents_Event;

namespace SlimeGame
{
    public enum ExcelColumn 
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    public static class StatsCSVWriter
    {
        public static void SaveCompletionAndStatsCSV(FullGenerationStats[] fullStats,string folderPath,string name,bool addDateAndTime = true) {

            /// TODO: Add multipe Sheet functionality
            /// 
            /// create two seperate sheets
            ///      > sheet one >> 'presentation'
            ///          > compiled data
            ///          > graph using sheet 2 data
            ///      > sheet 2
            ///          > table of all data
            ///              > sorted by rows

            /// TODO: Set up table for easier graphing
            /// 
            /// stats to track
            /// outside stats (6)
            ///     > generation size
            ///     > generation volume
            ///     > cell counts
            ///         > void
            ///         > surface
            ///         > core
            ///         
            /// Tessera Stats (8)
            ///     > createHelperTime;
            ///     > initializeTime;
            ///     > createPropagatorTime;
            ///     > initialConstraintsTime;
            ///     > skyboxTime;
            ///     > banBigTilesTime;
            ///     > runTime;
            ///     > postProcessTime;
            ///     
            /// Tessera Completion (6)
            ///     > success
            ///     > tileData.Count
            ///     > retries
            ///     > backtrackCount
            ///     > contradictionLocation
            ///     > contradictionReason
            ///     
            /// Stats Values
            ///     > Mean                  =AVERAGE(A2:A6) /* look up average if too, may be useful later */
            ///     > Median                =MEDIAN(A2:A6)
            ///     > Modes                 =MODE.MULT(A2:A13)
            ///     > Modes Count
            ///     > Min                   =MIN(A2:A6)
            ///     > Max                   =MAX(A2:A6)
            ///     > Variance              =VAR(A2:A11) /* can also use S or P for sample & population */
            ///     > Standard Deviation    =STDEV.S(A2:A11) or =STDEV.P(A2:A11)
            ///     
            /// square root => =SQRT(ABS(A2)) OR =SQRT(A2)
            /// power => POWER(98.6,3.2)
            /// sum => SUM(A2:A11) OR SUM(A2:A11,C2:C11) /* for multiple areas */
            /// 
            /// 
            /// ( use ) sort by func =SORTBY(A2:A6,B2:B6) OR =SORTBY(tbl_/* table title */,tbl_/* table title */[/* column title to sort by*/],/* 1 || -1 => 1 for ascending && -1 for descending order *//* can repeat funtion here to sort by other columns after previous sorting is applied => same syntax */) 
            /// https://support.microsoft.com/en-us/office/sortby-function-cd2d7a62-1b93-435c-b561-d6a35134f28f
            /// 
            /// sheet management => https://support.microsoft.com/en-us/office/sheet-function-44718b6f-8b87-47a1-a9d6-b701c06cff24
            /// 
            /// t test ( not for this, but nice to know ) => T.TEST(array1,array2,tails,type)

            var rowData = new List<string[]>();
            var rowWithDataCount = fullStats.Length;
            var skipRowsCount = 2;


            /// TODO: Test below
            /// -> normalized data has not been changed from the original use with tile ratings
            /// -> not accurate for this application
            ///
            /// =AVERAGE(A2:A6) // ( ! ) look up AVERAGEIF too, may be useful later
            /// =MEDIAN(A2:A6)
            /// =MODE.MULT(A2:A13)
            /// =MIN(A2:A6)
            /// =MAX(A2:A6)
            /// =VAR(A2:A11) // ( ! ) can also use S or P ( sample or population respectively )
            /// =STDEV.S(A2:A11) or =STDEV.P(A2:A11)

            var columnToApply = ExcelColumn.M;

            /// + 3 is to offset for calcs at top of sheet
            /// + 1 is b/c excel starts at 1.....
            int rowOffset = skipRowsCount + 3 + 1;
            int lowerRowOffset = rowWithDataCount + rowOffset - 1;

            var meanString              = $"\"=AVERAGE({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var medianString            = $"\"=MEDIAN({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var modesString             = $"\"=MODE.MULT({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var minString               = $"\"=MIN({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var maxString               = $"\"=MAX({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var varianceString          = $"\"=VAR.S({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";
            var standardDeviationString = $"\"=STDEV.S({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset})\"";

            /// 2 b/c that is the row where the calculation values are at:
            /// -> mean located at A2
            /// -> standard_dev located at G2
            /// ( excel function ) -> =NORM.DIST(x,mean,standard_dev,Cumulative)
            var normValueString =  $"\"= NORM.DIST({columnToApply}{rowOffset}:{columnToApply}{lowerRowOffset},{ExcelColumn.A}2,{ExcelColumn.G}2,FALSE)\"";


            string[] titles = new []
            {
                "Mean",
                "Median",
                "Modes",
                "Min",
                "Max",
                "Variance",
                "Standard Deviation",
            };
            string[] values = new []
            {
                meanString,
                medianString,
                modesString,
                minString,
                maxString,
                varianceString,
                standardDeviationString,
            };
            rowData.Add(titles);
            rowData.Add(values);


            // to add space
            for(int i = 0; i < skipRowsCount; i++) 
            {
                rowData.Add(new string[] { "", });
            }


            var columnCount = 20;

            string[] columnTitles = new string[columnCount];
            columnTitles[0]     = $"Name";
            columnTitles[1]     = $"Size";
            columnTitles[2]     = $"Volume";
            columnTitles[3]     = $"Void Cells";
            columnTitles[4]     = $"Surface Cells";
            columnTitles[5]     = $"Air Cells";      
            columnTitles[6]     = $"Create Helper Time;";
            columnTitles[7]     = $"Initialize Time;";
            columnTitles[8]     = $"Create Propagator Time;";
            columnTitles[9]     = $"Initial Constraints Time";
            columnTitles[10]    = $"Skybox Time";
            columnTitles[11]    = $"Ban Big Tiles Time";
            columnTitles[12]    = $"Run Time";
            columnTitles[13]    = $"Post Process Time";
            columnTitles[14]    = $"Success";
            columnTitles[15]    = $"TileData Count";
            columnTitles[16]    = $"Retries";
            columnTitles[17]    = $"BacktrackCount";
            columnTitles[18]    = $"ContradictionLocation";
            columnTitles[19]    = $"ContradictionReason";
            // columnTitles[20]    = $"Normal Value"; /// Normal Value inacurate -> need to set values for this application
            rowData.Add(columnTitles);


            var statsByVolume = fullStats.OrderBy(x => x.Volume).ToArray();

            for(int i = 0; i < statsByVolume.Length; i++) 
            {
                string[] statValues = new string[columnCount];

                statValues[0]  = $"\"{statsByVolume[i].Name}\"";                                 /// Name
                statValues[1]  = $"\"{statsByVolume[i].Size}\"";                                 /// Size
                statValues[2]  = $"\"{statsByVolume[i].Volume}\"";                               /// Volume
                statValues[3]  = $"\"{statsByVolume[i].VoidCells}\"";                            /// Void Cells
                statValues[4]  = $"\"{statsByVolume[i].SurfaceCells}\"";                         /// Surface Cells
                statValues[5]  = $"\"{statsByVolume[i].CoreCells}\"";                            /// Air Cells

                statValues[6]  = $"\"{statsByVolume[i].Stats.createHelperTime}\"";               /// Create Helper Time
                statValues[7]  = $"\"{statsByVolume[i].Stats.initializeTime}\"";                 /// Initialize Time
                statValues[8]  = $"\"{statsByVolume[i].Stats.createPropagatorTime}\"";           /// Create Propagator Time
                statValues[9]  = $"\"{statsByVolume[i].Stats.initialConstraintsTime}\"";         /// Initial Constraints Time
                statValues[10] = $"\"{statsByVolume[i].Stats.skyboxTime}\"";                    /// Skybox Time
                statValues[11] = $"\"{statsByVolume[i].Stats.banBigTilesTime}\"";               /// Ban Big Tiles Time
                statValues[12] = $"\"{statsByVolume[i].Stats.runTime}\"";                       /// Run Time
                statValues[13] = $"\"{statsByVolume[i].Stats.postProcessTime}\"";               /// Post Process Time

                statValues[14] = $"\"{statsByVolume[i].Completion.success}\"";                  /// Success
                statValues[15] = $"\"{statsByVolume[i].Completion.tileData.Count}\"";           /// TileData Count
                statValues[16] = $"\"{statsByVolume[i].Completion.retries}\"";                  /// Retries
                statValues[17] = $"\"{statsByVolume[i].Completion.backtrackCount}\"";           /// BacktrackCount
                statValues[18] = $"\"{statsByVolume[i].Completion.contradictionLocation}\"";    /// ContradictionLocation
                statValues[19] = $"\"{statsByVolume[i].Completion.contradictionReason}\"";      /// ContradictionReason

                // statValues[20] = $"{normValueString}";  // Normal Value inacurate -> need to set values for this application

                rowData.Add(statValues);
            }


            string[][] output = new string[rowData.Count][];

            for(int i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            int length          = output.GetLength(0);
            string delimiter    = ",";

            StringBuilder sb = new StringBuilder();

            for(int index = 0; index < length; index++) 
            {
                sb.AppendLine(string.Join(delimiter,output[index]));
            }

            string filePath = GetDataPath();
            if(addDateAndTime)
            {
                filePath = filePath + "/" + folderPath + $" /{name} {SGUtils.DateTimeStamp()}.csv";
            } 
            else 
            {
                filePath = filePath + folderPath + $"/{name}.csv";
            }
            StreamWriter outStream = File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }


        /// Following method is used to retrive the relative path as device platform
        private static string GetDataPath() {
#if UNITY_EDITOR
            /// C:/Users/David/Unity Projects/Base Template - 2023.1.1f1/Assets
            return Application.dataPath;
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
        }

    }
}
