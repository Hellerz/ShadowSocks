namespace ZXing
{
    using System;

    [Flags]
    public enum BarcodeFormat
    {
        All_1D = 0xf1de,
        AZTEC = 1,
        CODABAR = 2,
        CODE_128 = 0x10,
        CODE_39 = 4,
        CODE_93 = 8,
        DATA_MATRIX = 0x20,
        EAN_13 = 0x80,
        EAN_8 = 0x40,
        ITF = 0x100,
        MAXICODE = 0x200,
        MSI = 0x20000,
        PDF_417 = 0x400,
        PLESSEY = 0x40000,
        QR_CODE = 0x800,
        RSS_14 = 0x1000,
        RSS_EXPANDED = 0x2000,
        UPC_A = 0x4000,
        UPC_E = 0x8000,
        UPC_EAN_EXTENSION = 0x10000
    }
}

