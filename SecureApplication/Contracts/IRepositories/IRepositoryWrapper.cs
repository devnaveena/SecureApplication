namespace Contracts.IRepositories
{
    public interface IRepositoryWrapper
    {
        //This property can be used to access the methods and properties of the `IBookRepository` interface. 
        IBookRepository Book { get; }
        
        // This property can be used to access the methods and properties of the `IAccountRepository` interface. 
        IAccountRepository Account { get; }

        /// <summary>
        /// The function "Save"  saves all changes to database.
        /// </summary>
        void Save();
    }
}