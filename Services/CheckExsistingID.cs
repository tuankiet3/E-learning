namespace E_learning.Services
{
    public class CheckExsistingID
    {
        public async Task<string> GenerateUniqueID<TModel>(Func<Task<List<TModel>>> getAllFunc,Func<TModel, string> getIDFunc, Func<string> generateIDFunc)
        {
            var existingModels = await getAllFunc();
            var existingIDs = existingModels
                .Select(getIDFunc)
                .ToHashSet();

            string newID;
            do
            {
                newID = generateIDFunc();
            }
            while (existingIDs.Contains(newID));

            return newID;
        }
    }
}
