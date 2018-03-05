using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nethereum.Web3;
using System.IO;
using Nethereum.RPC.Eth.DTOs;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Nethereum.Contracts;
using System.Numerics;
using ICOPlatformV1.ActionFilters;
using Nethereum.RPC.Accounts;
using ICOPlatformV1.Models;
using Nethereum.ABI.FunctionEncoding;

namespace ICOPlatformV1.Controllers
{
  public class ContractController : Controller
  {
    private readonly Web3 web3;
    private readonly IHostingEnvironment environment;
    private readonly IAccount account;

    public ContractController(Web3 web3, IHostingEnvironment environment, IAccount account)
    {
      this.web3 = web3;
      this.environment = environment;
      this.account = account;
    }

    public IActionResult Index()
    {
      List<SelectListItem> items = new List<SelectListItem>();
      items.Add(new SelectListItem { Text = "0x35ff31858a696c736c96f092a0b9164748234b76", Value = "0x35ff31858a696c736c96f092a0b9164748234b76", Selected = true });
      ViewBag.ContractList = items;
      return View();
    }

    [HttpPost]
    //[ContractFilter]
    public IActionResult SetContract()
    {
      var id = Request.Form["ContractList"].ToString();
      HttpContext.Session.SetString("contractid", id);
      return RedirectToAction("Detail");
    }

    [ContractFilter]
    public async Task<IActionResult> Detail()
    {
      Contract contract = new Logic(web3, environment).GetContract(HttpContext.Session.GetString("contractid"));

      var totaleth = await contract.GetFunction("getBalance").CallAsync<BigInteger>();
      var totalPayCount = await contract.GetFunction("getPayCount").CallAsync<BigInteger>();

      ViewBag.totaleth = new Nethereum.Util.UnitConversion().FromWei(totaleth);
      ViewBag.totalPayCount = totalPayCount;

      return View("Detail");
    }

    [HttpPost]
    [ContractFilter]
    public async Task<IActionResult> AddWL()
    {
      string address = Request.Form["address"].ToString();
      double min = Convert.ToDouble(Request.Form["min"]);
      double max = Convert.ToDouble(Request.Form["max"]);

      var minwei = new Nethereum.Util.UnitConversion().ToWei(min);
      var maxwei = new Nethereum.Util.UnitConversion().ToWei(max);

      var contract = new Logic(web3, environment).GetContract(HttpContext.Session.GetString("contractid"));
      var addwl = contract.GetFunction("addWhiteList");
      var transactionHash = await addwl.SendTransactionAsync(account.Address, address, minwei, maxwei);
      //var receipt = await MineAndGetReceiptAsync(web3, transactionHash);

      TempData["result"] = "success";
      return PartialView("AddWL");
    }

    [HttpPost]
    public async Task<IActionResult> LockWL()
    {
      try
      {
        var process = Request.Form["chkLockWl"] == "on" ? true : false;
        string address = Request.Form["lockAddress"].ToString();
        var contract = new Logic(web3, environment).GetContract(HttpContext.Session.GetString("contractid"));
        Function f;
        byte[] param;
        if (process == true)
        {
          f = contract.GetFunction("activateWL");

          //param = new FunctionCallEncoder().EncodeParameters(contract.ContractBuilder.GetFunctionBuilder("activateWL").FunctionABI.InputParameters, contract.ContractBuilder.GetFunctionBuilder("activateWL").FunctionABI.Sha3Signature.Replace("0x", ""), address);
        }
        else
        {
          f = contract.GetFunction("lockWL");
          //param = new Nethereum.ABI.Encoders.StringTypeEncoder().Encode(f.CreateCallInput(address).Data);
        }

        new MultiSigController(this.web3, this.environment, this.account).SendTransaction(HttpContext.Session.GetString("contractid"), f.CreateCallInput(address).Data);
      }
      catch (Exception e)
      {

        throw;
      }
      return PartialView("Detail");
    }

    [HttpPost]
    public ActionResult GetWLDetails(string address)
    {

      var contract = new Logic(web3, environment).GetContract(HttpContext.Session.GetString("contractid"));
      var getWhiteListDetails = contract.GetFunction("getWhiteListDetails");
      var result = getWhiteListDetails.CallDeserializingToObjectAsync<WLType>(address);
      Task.WaitAll(result);
      return PartialView("WLDetails", result.Result);
    }

    public async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3 web3, string transactionHash)
    {
      var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

      while (receipt == null)
      {
        Thread.Sleep(1000);
        receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
      }
      return receipt;
    }

  }
}