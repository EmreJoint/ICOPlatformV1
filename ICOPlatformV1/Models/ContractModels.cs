using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ICOPlatformV1.Models
{
  [FunctionOutput]
  public class WLType
  {
    [Parameter("address", "addr", 1)]
    public string adress { get; set; }

    [Parameter("bool", "active", 2)]
    public bool active { get; set; }

    [Parameter("uint256", "amount", 3)]
    public BigInteger amount { get; set; }

    [Parameter("uint256", "min", 4)]
    public BigInteger min { get; set; }

    [Parameter("uint256", "max", 5)]
    public BigInteger max { get; set; }

    [Parameter("bool", "flag", 6)]
    public bool flag { get; set; }
  }
}
