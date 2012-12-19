using System;

namespace SqlDataReaderToObject.Tests.DTOs
{
    public class AllTheTypes
    {
        public int TheInt;
        public string TheVarChar;
        public string TheNVarChar;
        public string TheText;
        public string TheNText;
        public string TheXml;
        public long TheBigInt;
        public Guid TheUniqueIdentifier;
        public bool TheBit;
        public byte[] TheBinary = new byte[0];
        public byte[] TheVarBinary = new byte[0];
        public byte[] TheImage = new byte[0];
        public double TheDecimal;
        public DateTime TheDate = new DateTime(2000, 1, 1);
        public DateTime TheSmallDateTime = new DateTime(2000, 1, 1);
        public DateTime TheDateTime = new DateTime(2000, 1, 1);
        public DateTime TheDateTime2 = new DateTime(2000, 1, 1);
        public DateTime TheDateTimeOffset = new DateTime(2000, 1, 1);
        public long TheTimeStamp;
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
        public double TheDecimal;
        public DateTime? TheDate;
        public DateTime? TheSmallDateTime;
        public DateTime? TheDateTime;
        public DateTime? TheDateTime2;
        public DateTime? TheDateTimeOffset;
        public long? TheTimeStamp;
    }

}
