using System;

namespace SqlDataReaderToObject.Tests.DTOs
{
    public class AllTheTypes
    {
        public int TheInt = 123;
        public string TheVarChar = "a";
        public string TheNVarChar = "b";
        public string TheText = "c";
        public string TheNText = "d";
        public string TheXml = "e";
        public long TheBigInt = 456; 
        public Guid TheUniqueIdentifier = new Guid(1,2,3, new byte[8]);
        public bool TheBit = true;
        public byte[] TheBinary = new byte[] { 123 };
        public byte[] TheVarBinary = new byte[] { 123};
        public byte[] TheImage = new byte[] { 234};
        public decimal TheDecimal = (decimal)10;
        public DateTime TheDate = new DateTime(2000, 1, 1);
        public DateTime TheSmallDateTime = new DateTime(2000, 1, 1);
        public DateTime TheDateTime = new DateTime(2000, 1, 1);
        public DateTime TheDateTime2 = new DateTime(2000, 1, 1);
        public DateTimeOffset TheDateTimeOffset = new DateTimeOffset(1,1,1,1,1,1,1,TimeSpan.FromHours(1));
        public byte[] TheTimeStamp;
    }

    public class AllTheNullableTypes
    {
        public int? TheInt;
        public string TheVarChar;
        public string TheNVarChar;
        public string TheText;
        public string TheNText;
        public string TheXml;
        public long? TheBigInt;
        public Guid? TheUniqueIdentifier;
        public bool? TheBit;
        public byte[] TheBinary;
        public byte[] TheVarBinary;
        public byte[] TheImage;
        public decimal TheDecimal;
        public DateTime? TheDate;
        public DateTime? TheSmallDateTime;
        public DateTime? TheDateTime;
        public DateTime? TheDateTime2;
        public DateTimeOffset? TheDateTimeOffset;
        public byte[] TheTimeStamp;
    }

}
