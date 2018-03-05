using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Contracts;
using Nethereum.RPC.Accounts;
using Nethereum.Web3;
using Microsoft.AspNetCore.Http;
using System.Text;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;

namespace ICOPlatformV1.Controllers
{
  public class MultiSigController : Controller
  {
    private readonly Web3 web3;
    private readonly IHostingEnvironment environment;
    private readonly IAccount account;

    public MultiSigController(Web3 web3, IHostingEnvironment environment, IAccount account)
    {
      this.web3 = web3;
      this.environment = environment;
      this.account = account;
    }

    public void SendTransaction(string to, string s)
    {
      try
      {
        Contract contract = new Logic(web3, environment).GetContract("0x2ab74b1d842e47ea63d5241a6ed33a3db9f6e1d2");
        //var by = contract.ContractBuilder.GetFunctionBuilder("execute").FunctionABI.InputParameters[2].ABIType.Encode(s);
        var execute = contract.GetFunction("execute");
        var transactionHash = execute.SendTransactionAsync(account.Address, to, 0, s.HexToByteArray());
        Task.WaitAll(transactionHash);
      }
      catch (Exception e)
      {
        throw new Exception(e.Message);
      }
    }
  }
}