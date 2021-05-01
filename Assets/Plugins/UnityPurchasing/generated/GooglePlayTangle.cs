#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("/03O7f/CycblSYdJOMLOzs7Kz8xZeDwv1M3GCeteHreIDl3XptZTY9aG0o97iK0IHuiNx76mCCRSPrK2cPTopewS+ntHo0SksaB9B0WcAuWMP2iVJi0NU8HNje5trfaioGQFrxBI7x+dUiMYElqM6s6HvfhjZZsEI0xv3p3K4nZaga1hlV1YoP2OWaApYbx3UPWcqQ4hHMEyiKWHKOC6Mz04PbPHaOqQEwtpFz3YkPULkHKYEiX+4wD9q1Eb87vLkk7vApxfNuPCz2qm3TqmkdKjClen9tj26jtH/7+PMBpmTU+L8EQ9AvM7C3mCeG4BTc7Az/9NzsXNTc7OzyAetdaG38KuNT4T5BYM+vlXxSXaNRMsclbLmeeLbOwyuUSIaM3Mzs/O");
        private static int[] order = new int[] { 0,2,11,6,4,9,6,8,9,10,12,13,13,13,14 };
        private static int key = 207;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
