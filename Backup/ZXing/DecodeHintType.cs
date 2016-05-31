namespace ZXing
{
    using System;

    public enum DecodeHintType
    {
        OTHER,
        PURE_BARCODE,
        POSSIBLE_FORMATS,
        TRY_HARDER,
        CHARACTER_SET,
        ALLOWED_LENGTHS,
        ASSUME_CODE_39_CHECK_DIGIT,
        NEED_RESULT_POINT_CALLBACK,
        ASSUME_MSI_CHECK_DIGIT,
        USE_CODE_39_EXTENDED_MODE,
        RELAXED_CODE_39_EXTENDED_MODE,
        TRY_HARDER_WITHOUT_ROTATION,
        ASSUME_GS1,
        RETURN_CODABAR_START_END,
        ALLOWED_EAN_EXTENSIONS
    }
}

