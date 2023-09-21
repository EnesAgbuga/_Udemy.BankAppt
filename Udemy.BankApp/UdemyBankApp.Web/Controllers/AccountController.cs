using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using UdemyBankApp.Web.Data.Context;
using UdemyBankApp.Web.Data.Entities;
using UdemyBankApp.Web.Data.Interfaces;
using UdemyBankApp.Web.Data.Repositories;
using UdemyBankApp.Web.Data.UnitOfWork;
using UdemyBankApp.Web.Mapping;
using UdemyBankApp.Web.Models;

namespace UdemyBankApp.Web.Controllers
{
    public class AccountController : Controller
    {
        //private readonly BankContext _context;
        //private readonly IApplicationUserRepository _applicationUserRepository; 
        //private readonly IUserMapper _userMapper;
        //private readonly IAccountRepository _accountRepository;
        //private readonly IAccountMapper _accountMapper;

        //public AccountController(/*BankContext context,*/ IApplicationUserRepository applicationUserRepository,
        //    IUserMapper userMapper , IAccountRepository accountRepository, IAccountMapper accountMapper )
        //{
        //    //_context = context;
        //    _applicationUserRepository = applicationUserRepository;
        //    _userMapper = userMapper;
        //    _accountRepository = accountRepository;
        //    _accountMapper = accountMapper;
        //}

        //private readonly IRepository<Account> _accountRepository;
        //private readonly IRepository<ApplicationUser> _applicationUserRepository;

        //public AccountController(IRepository<Account> accountRepository, IRepository<ApplicationUser> applicationUserRepository)
        //{
        //    _accountRepository = accountRepository;
        //    _applicationUserRepository = applicationUserRepository;
        //}

        private readonly IUow _uow;

        public AccountController(IUow uow)
        {
            _uow = uow;
        }

        public IActionResult Create(int id)
        {
            //var userInfo = _userMapper.MapToUserList( _applicationUserRepository.GetbById(id));
            var userInfo = _uow.GetRepository<ApplicationUser>().GetById(id);
            return View(new UserListModel
            {
                Id = userInfo.Id,
                Name = userInfo.Name,
                Surname = userInfo.Surname
            });
        }

        [HttpPost]
        public IActionResult Create(AccountCreateModel model)
        {

            _uow.GetRepository<Account>().Create(new Account
            {
                AccountNumber = model.AccountNumber,
                Balance = model.Balance,
                ApplicationUserId = model.ApplicationUserId
            });
            _uow.SaveChanges();
            //_accountRepository.Create(_accountMapper.Map(model));2
            //_context.Accounts.Add(new Data.Entities.Account
            //{
            //    AccountNumber = model.AccountNumber,
            //    ApplicationUserId = model.ApplicationUserId,
            //    Balance = model.Balance
            //});
            //_context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult GetByUserId(int userId)
        {
            var query = _uow.GetRepository<Account>().GetQueryable();
            var accounts = query.Where(x => x.ApplicationUserId == userId).ToList();

            var user = _uow.GetRepository<ApplicationUser>().GetById(userId);
            ViewBag.FullName = user.Name + " " + user.Surname;


            var list = new List<AccountListModel>();

            foreach (var account in accounts)
            {
                list.Add(new AccountListModel
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    ApplicationUserId = account.ApplicationUserId,
                    Id = account.Id
                });
            }
            return View(list);

        }
        [HttpGet]
        public IActionResult SendMoney(int accountId)
        {

            var query = _uow.GetRepository<Account>().GetQueryable();
            var accounts = query.Where(x => x.Id != accountId).ToList();
            var list = new List<AccountListModel>();
            ViewBag.SenderId = accountId;
            foreach (var account in accounts)
            {
                list.Add(new()
                {
                    AccountNumber = account.AccountNumber,
                    ApplicationUserId = account.ApplicationUserId,
                    Balance = account.Balance,
                    Id = account.Id
                });
            }
            var list2 = new SelectList(list);

            return View(new SelectList(list, "Id", "AccountNumber"));
        }
        [HttpPost]

        public IActionResult SendMoney(SendMoneyModel model)
        {
            var senderAccount = _uow.GetRepository<Account>().GetById(model.SenderId);
            senderAccount.Balance -= model.Amount;

            _uow.GetRepository<Account>().Update(senderAccount);

            var account = _uow.GetRepository<Account>().GetById(model.AccountId);
            account.Balance -= model.Amount;
            _uow.GetRepository<Account>() .Update(account);
            _uow.SaveChanges();

            return RedirectToAction("Index","Home");
        }

    }
}
