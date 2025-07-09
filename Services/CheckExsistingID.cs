namespace E_learning.Services
{
    public class CheckExsistingID
    {
        public async Task<string> GenerateUniqueIDForStringList(
                     Func<Task<List<string>>> getAllIDsFunc,
                     Func<string> generateIDFunc)
                        {
            var existingIDs = (await getAllIDsFunc()).ToHashSet();
            string newID;
            do
            {
                newID = generateIDFunc();
            } while (existingIDs.Contains(newID));

            return newID;
        }

    }
}
