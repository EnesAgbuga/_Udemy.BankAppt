using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyBankApp.Web.Models
{
    public class SendMoneyModel
    {
        /*  
            
            SenderId 
            AccountId
            Amount 
            
         */

        public int SenderId { get; set; }
        public int AccountId { get; set; }
        public int Amount { get; set; }
    }
}
