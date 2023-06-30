using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LABTOOLS.API.DataTransferObjects
{
    public class AnalyzerDTO : IDataTransferObject
    {
        public int Id { get; set; }
        
         public string? Name { get; set; }

         public int? ManufacturerId { get; set; }

         public string? Port { get; set; }
         public int? Baud { get; set; }
         public int? Parity { get; set; }
         public int? DataBits { get; set; }
         public int? StopBits { get; set; }
         public bool? XOn { get; set; }
         public bool? NullModenRequired { get; set; }
         public int? AnalyzerCode { get; set; }
         public int? AnalyzerHostCode { get; set; }
         public int? UploadCode { get; set; }
         public string? Template { get; set; }
         public string? AnBin { get; set; }
         public int? UserSpec { get; set; }
         public string? Adapter { get; set; }
         public string? ProtocolB { get; set; }
         public string? ProtocolD { get; set; }
         public string? Install { get; set; }
         public string? Operate { get; set; }
         public string? History { get; set; }
         public string? Transmit { get; set; }
         public string? Control { get; set; }
         public string? Barcode { get; set; }
         public string? Important { get; set; }
         public string? AssemblyFilename { get; set; }
         public string? SpecifigFilename { get; set; }
         public string? UploadType { get; set; }
         public string? CommunicatorType { get; set; }
         public string? OtherSettings { get; set; }
         public bool? Unidirectional { get; set; }
         public bool? Bidirectional { get; set; }
         public bool? FileBased { get; set; }
         public bool? FileBasedBidirectional { get; set; }
         public bool? HostQuery { get; set; }
         public bool? Microbiology { get; set; }
         public bool? Middleware { get; set; }
         public bool? LdComm { get; set; }
         public bool? ldMediator { get; set; }
         public bool? CgmLabdaq { get; set; }
         
         public DateTime? ModifiedDate { get; set; }
         public int? ModifiedByUserId { get; set; }
         public string? ModifiedByUserName { get; set; }
    }
}