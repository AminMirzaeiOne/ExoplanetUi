using System.Globalization;

namespace Exoplanet.UI
{
    public static class CultureInfos
    {
        // Language ID Reference:
        // https://docs.microsoft.com/en-us/previous-versions/windows/embedded/ms912047(v=winembedded.10)?redirectedfrom=MSDN
        // https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c
        // ID  Language
        // 1025 Arabic
        // 1041 Japanese
        // 1028 Traditional Chinese
        // 1042 Korean
        // 1029 Czech
        // 1043 Dutch
        // 1030 Danish
        // 1044 Norwegian
        // 1031 German
        // 1045 Polish
        // 1032 Greek
        // 1046 Portuguese - Brazil
        // 1033 English
        // 1049 Russian
        // 1034 Spanish
        // 1053 Swedish
        // 1035 Finnish
        // 1054 Thai
        // 1036 French
        // 1055 Turkish
        // 1037 Hebrew
        // 2052 Simplified Chinese
        // 1038 Hungarian
        // 2070 Portuguese
        // 1040 Italian

        /// <summary>
        /// 2052 Simplified Chinese
        /// </summary>
        public const int LCID_zh_CN = 2052;

        /// <summary>
        /// 1028 Traditional Chinese
        /// </summary>
        public const int LCID_zh_TW = 1028;

        /// <summary>
        /// 1033 English
        /// </summary>
        public const int LCID_en_US = 1033;

        /// <summary>
        /// 2052 Simplified Chinese
        /// </summary>
        public static readonly CultureInfo zh_CN = CultureInfo.GetCultureInfo(LCID_zh_CN);

        /// <summary>
        /// 1028 Traditional Chinese
        /// </summary>
        public static readonly CultureInfo zh_TW = CultureInfo.GetCultureInfo(LCID_zh_TW);

        /// <summary>
        /// 1033 English
        /// </summary>
        public static readonly CultureInfo en_US = CultureInfo.GetCultureInfo(LCID_en_US);

    }
}
