using MultiAlignCore.IO.InputFiles;

namespace MultiAlignRogue.AMTMatching
{
    internal sealed class DatabaseDesignTimeDataViewModel : DatabasesViewModel
    {
        public DatabaseDesignTimeDataViewModel() : base()
        {
            AddDatabase(new InputDatabase()
            {
                DatabaseName = "MT_The_Mouse_The_Cat_Brought_In",
                DatabaseServer = "Willy",
                Description = "It looked a little sick, so we sampled it.",
                Organism = "Mus_musculus",
            });
            AddDatabase(new InputDatabase()
            {
                DatabaseName = "MT_The_Dogs_Toenails",
                DatabaseServer = "Johnny",
                Description = "Some toenail clippings from the dog",
                Organism = "Woofus_scratcherus",
            });
            AddDatabase(new InputDatabase()
            {
                DatabaseName = "MT_Moldy_Ham",
                DatabaseServer = "Maddy",
                Description = "Some moldy ham from the back of the fridge",
                Organism = "Porkus_rainbowus",
            });
        }
    }
}
