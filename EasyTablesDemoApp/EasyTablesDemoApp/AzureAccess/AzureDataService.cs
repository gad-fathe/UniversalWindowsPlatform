using EasyTablesDemoApp.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTablesDemoApp.AzureAccess
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable<User> usersTable;



        public async Task Initialize()
        {
            //Create MobileService reference with the back-end address:
            MobileService = new MobileServiceClient("https://easytablesdemo.azurewebsites.net");

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<User>();
            await App.MobileService.SyncContext.InitializeAsync(store);

            //Get our sync table that will call out to azure
            usersTable = MobileService.GetSyncTable<User>();
        }

        public async Task<List<User>> GetUsers()
        {
            await SyncUsers();
            return await usersTable.ToListAsync();
        }

        public async Task AddUser()
        {
            var user = new User
            {
                First_name = "Daniel",
                Last_name = "Krzyczkowski",
                Email = "mobileprogrammer@..."
            };

            await usersTable.InsertAsync(user);

            //Synchronize coffee
            await SyncUsers();
        }
        // Azure automatically syncs our local database and the backend when connectivity is reestablished:
        public async Task SyncUsers()
        {
            await usersTable.PullAsync("users", usersTable.CreateQuery());
            await App.MobileService.SyncContext.PushAsync();
        }
    }
}
