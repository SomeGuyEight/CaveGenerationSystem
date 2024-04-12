
namespace SlimeGame
{
    public class TesseraModifications
    {



        /// <see cref="Tessera.TesseraStats"/> => from internal => public
        //
        ///     => implemented by 
        ///     
        ///         1. <see cref="FullGenerationStats"/>
        ///         2. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        ///             => <see cref="GenerationManager.AddGenerationStats"/>
        ///                 => <see cref="StatsCSVWriter.SaveCompletionAndStatsCSV"/>
        ///



        /// <see cref="Tessera.TesseraGenerateOptions"/> => add action
        // 
        ///     => implemented by 
        ///     
        ///         1. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        //
        ///     => add this Action anywhere in TesseraGenerateOptions
        ///     
        ///         public Action<TesseraStats> returnStats;



        /// <see cref="Tessera.TesseraGenerator.StartGenerateInner"/> => add implementation of returnStats action
        // 
        ///     => implemented by
        ///     
        ///         1. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        //
        ///     => add these lines to TesseraGenerator.StartGenerateInner ~line 540 
        ///     
        ///         var stats = generatorHelper.Stats;
        ///         options.returnStats?.Invoke(stats);



    }
}
