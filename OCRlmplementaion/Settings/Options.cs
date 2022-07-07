namespace PoiskIT.Andromeda.Settings
{
    public enum QualityEnum
    {
        fast,
        def,
        best
    }
    public class Options
    {
        public string[] Languages { get; set; }
        public QualityEnum Quality { get; set; }
        public bool IsScaling { get; set; }
        public bool IsDenoising { get; set; }
        public bool IsFilter2D { get; set; }
        public bool IsGaussianWeighted { get; set; }
        public bool IsBilateral { get; set; }
        public bool IsDirectory { get; set; }
        public Options(string[] languages,
            QualityEnum quality,
            bool isScaling = false,
            bool isDenoising = false,
            bool isFilter2D = false,
            bool isGaussianWeighted = false,
            bool isBilateral = false)
        {
            Languages = languages;
            Quality = quality;
            IsScaling = isScaling;
            IsDenoising = isDenoising;
            IsFilter2D = isFilter2D;
            IsGaussianWeighted = isGaussianWeighted;
            IsBilateral = isBilateral;
            IsDirectory = false;
        }

        public static Options Fast
        {
            get
            {
                var languages = new[] { "rus", "eng", "deu", "fra", "spa", "chi_sim", "chi_tra", "jpn", "ara", "tur", "heb" };
                return new Options(languages, QualityEnum.fast);
            }
        }
        public static Options Default
        {
            get
            {
                var languages = new[] { "rus", "eng", "deu", "fra", "spa", "chi_sim", "chi_tra", "jpn", "ara", "tur", "heb" };
                return new Options(languages, QualityEnum.def);
            }
        }

        public static Options Best
        {
            get
            {
                // Do not has "spa" and "tur"
                var languages = new[] { "rus", "eng", "deu", "fra", "chi_sim", "chi_tra", "jpn", "ara", "heb" };
                return new Options(languages, QualityEnum.best);
            }
        }
        /*
        public static Options Fast
        {
            get
            {
                var languages = new[] { "afr", "amh", "ara", "asm", "aze", "aze_cyrl", "bel", "ben", "bod", "bos", "bre", "bul",
                    "cat", "ceb", "ces", "chi_sim", "chi_sim_vert", "chi_tra", "chi_tra_vert", "chr", "cos", "cym", "dan", "deu",
                    "div", "dzo", "ell", "eng", "enm", "epo", "equ", "est", "eus", "fao", "fas", "fil", "fin", "fra", "frk", "frm",
                    "fry", "gla", "gle", "glg", "grc", "guj", "hat", "heb", "hin", "hrv", "hun", "hye", "iku", "ind", "isl", "ita",
                    "ita_old", "jav", "jpn", "jpn_vert", "kan", "kat", "kat_old", "kaz", "khm", "kir", "kmr", "kor", "kor_vert",
                    "lao", "lat", "lav", "lit", "ltz", "mal", "mar", "mkd", "mlt", "mon", "mri", "msa", "mya", "nep", "nld", "nor",
                    "oci", "ori", "osd", "pan", "pol", "por", "pus", "que", "ron", "rus", "san", "sin", "slk", "slv", "snd", "spa",
                    "spa_old", "sqi", "srp", "srp_latn", "sun", "swa", "swe", "syr", "tam", "tat", "tel", "tgk", "tha", "tir", "ton",
                    "tur", "uig", "ukr", "urd", "uzb", "uzb_cyrl", "vie", "yid", "yor" };
                return new Options(languages, QualityEnum.fast);
            }
        }
        public static Options Default
        {
            get
            {
                var languages = new[] { "afr", "amh", "ara", "asm", "aze", "aze_cyrl", "bel", "ben", "bod", "bos", "bre",
                    "bul", "cat", "ceb", "ces", "chi_sim", "chi_sim_vert", "chi_tra", "chi_tra_vert", "chr", "cos", "cym",
                    "dan", "dan_frak", "deu", "deu_frak", "div", "dzo", "ell", "eng", "enm", "epo", "equ", "est", "eus",
                    "fao", "fas", "fil", "fin", "fra", "frk", "frm", "fry", "gla", "gle", "glg", "grc", "guj", "hat", "heb",
                    "hin", "hrv", "hun", "hye", "iku", "ind", "isl", "ita", "ita_old", "jav", "jpn", "jpn_vert", "kan", "kat",
                    "kat_old", "kaz", "khm", "kir", "kmr", "kor", "kor_vert", "lao", "lat", "lav", "lit", "ltz", "mal", "mar",
                    "mkd", "mlt", "mon", "mri", "msa", "mya", "nep", "nld", "nor", "oci", "ori", "osd", "pan", "pol", "por",
                    "pus", "que", "ron", "rus", "san", "sin", "slk", "slk_frak", "slv", "snd", "spa", "spa_old", "sqi", "srp",
                    "srp_latn", "sun", "swa", "swe", "syr", "tam", "tat", "tel", "tgk", "tgl", "tha", "tir", "ton", "tur",
                    "uig", "ukr", "urd", "uzb", "uzb_cyrl", "vie", "yid", "yor" };
                return new Options(languages, QualityEnum.def);
            }
        }

        public static Options Best
        {
            get
            {
                var languages = new[] { "afr", "amh", "ara", "asm", "aze", "aze_cyrl", "bel", "ben", "bod", "bos", "bre", "bul",
                    "cat", "ceb", "ces", "chi_sim", "chi_sim_vert", "chi_tra", "chi_tra_vert", "chr", "cos", "cym", "dan", "deu",
                    "div", "dzo", "ell", "eng", "enm", "epo", "est", "eus", "fao", "fas", "fil", "fin", "fra", "frk", "frm", "fry",
                    "gla", "gle", "glg", "grc", "guj", "hat", "heb", "hin", "hrv", "hun", "hye", "iku", "ind", "isl", "ita", "ita_old",
                    "jav", "jpn", "jpn_vert", "kan", "kat", "kat_old", "kaz", "khm", "kir", "kmr", "kor", "kor_vert", "lat", "lav",
                    "lit", "ltz", "mal", "mar", "mkd", "mlt", "mon", "mri", "msa", "mya", "nep", "nld", "nor", "oci", "ori", "osd",
                    "pan", "pol", "por", "pus", "que", "ron", "rus", "san" };
                return new Options(languages, QualityEnum.def);
            }
        }
        */
    }
}
