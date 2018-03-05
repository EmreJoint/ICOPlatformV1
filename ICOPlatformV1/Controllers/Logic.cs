using Microsoft.AspNetCore.Hosting;
using Nethereum.Contracts;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ICOPlatformV1.Controllers
{
  public class Logic
  {
    private readonly Web3 web3;
    private readonly IHostingEnvironment environment;

    public Logic(Web3 web3, IHostingEnvironment environment)
    {
      this.web3 = web3;
      this.environment = environment;
    }

    public Contract GetContract(string contractid)
    {
      var path = Path.Combine(environment.ContentRootPath, $@"Abi\{contractid}.txt");
      string abi = File.ReadAllText(path);
      var contract = web3.Eth.GetContract(abi, contractid);
      return contract;
    }
  }
}
