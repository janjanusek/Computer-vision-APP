namespace ImageClassifierApp.Services.Normalization
{
    /// <summary>
    /// Class used for normalization of input array stream
    /// </summary>
    public class NormalizerService
    {
        private readonly float _oldMin;
        private readonly float _oldRange;
        private readonly float _newMin;
        private readonly float _newRange;

        public NormalizerService(float paOldMin, float paOldMax, float paNewMin, float paNewMax)
        {
            _oldMin = paOldMin;
            _oldRange = paOldMax - paOldMin;
            _newMin = paNewMin;
            _newRange = paNewMax - paNewMin;
        }

        public float[][] Normalize(byte[][] paBytes, float[][] paReturnArray = null)
        {
            var normalized = paReturnArray ?? new float[paBytes.Length][];
            for (int i = 0; i < paBytes.Length; i++)
            {
                normalized[i] = new float[paBytes.Length];
                float scale;
                for (int j = 0; j < paBytes[i].Length; j++)
                {
                    //where in the old scale is this value (0...1)
                    scale = (paBytes[i][j] - _oldMin) / _oldRange;
                    //place this scale in the new range
                    normalized[i][j] = (_newRange * scale) + _newMin;
                }
            }
            return normalized;
        }

        private TArrayType[][] InitArray<TArrayType>(int paArrays, int paLength)
        {
            var arrays = new TArrayType[paArrays][];
            for (int i = 0; i < paArrays; i++)
                arrays[i] = new TArrayType[paLength];
            return arrays;
        }

        public byte[][] Denormalize(float[][] paNormalized, byte[][] paByteArrays = null)
        {
            var bytes = paByteArrays ?? this.InitArray<byte>(paNormalized.Length, paNormalized[0].Length);
            float scale;
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < bytes[0].Length; j++)
                {
                    scale = (paNormalized[i][j] - _newMin) / _newRange;
                    bytes[i][j] = (byte)((_oldRange * scale) + _oldMin);
                }
            }
            return bytes;
        }

        public float[] Normalize(byte[] paBytes)
        {
            var normalized = new float[paBytes.Length];
            float scale;
            for (int i = 0; i < normalized.Length; i++)
            {
                //where in the old scale is this value (0...1)
                scale = (paBytes[i] - _oldMin) / _oldRange;
                //place this scale in the new range
                normalized[i] = (_newRange * scale) + _newMin;
            }
            return normalized;
        }

        public float[] NormalizeFloat(float[] paBytes)
        {
            var normalized = new float[paBytes.Length];
            float scale;
            for (int i = 0; i < normalized.Length; i++)
            {
                //where in the old scale is this value (0...1)
                scale = (paBytes[i] - _oldMin) / _oldRange;
                //place this scale in the new range
                normalized[i] = (_newRange * scale) + _newMin;
            }
            return normalized;
        }

        public byte[] Denormalize(float[] paNormalizedData)
        {
            var denormalized = new byte[paNormalizedData.Length];
            float scale = 0;
            for (int i = 0; i < denormalized.Length; i++)
            {
                scale = (paNormalizedData[i] - _newMin) / _newRange;
                denormalized[i] = (byte)((_oldRange * scale) + _oldMin);
            }
            return denormalized;
        }
    }
}
