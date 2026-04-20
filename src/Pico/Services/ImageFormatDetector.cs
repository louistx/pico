namespace Pico.Services;

public static class ImageFormatDetector
{
    public static bool TryDetectExtension(byte[] bytes, out string extension)
    {
        if (bytes.Length >= 8 &&
            bytes[0] == 0x89 &&
            bytes[1] == 0x50 &&
            bytes[2] == 0x4E &&
            bytes[3] == 0x47 &&
            bytes[4] == 0x0D &&
            bytes[5] == 0x0A &&
            bytes[6] == 0x1A &&
            bytes[7] == 0x0A)
        {
            extension = "png";
            return true;
        }

        if (bytes.Length >= 3 &&
            bytes[0] == 0xFF &&
            bytes[1] == 0xD8 &&
            bytes[2] == 0xFF)
        {
            extension = "jpg";
            return true;
        }

        if (bytes.Length >= 6 &&
            bytes[0] == 0x47 &&
            bytes[1] == 0x49 &&
            bytes[2] == 0x46 &&
            bytes[3] == 0x38)
        {
            extension = "gif";
            return true;
        }

        if (bytes.Length >= 2 &&
            bytes[0] == 0x42 &&
            bytes[1] == 0x4D)
        {
            extension = "bmp";
            return true;
        }

        if (bytes.Length >= 12 &&
            bytes[0] == 0x52 &&
            bytes[1] == 0x49 &&
            bytes[2] == 0x46 &&
            bytes[3] == 0x46 &&
            bytes[8] == 0x57 &&
            bytes[9] == 0x45 &&
            bytes[10] == 0x42 &&
            bytes[11] == 0x50)
        {
            extension = "webp";
            return true;
        }

        extension = string.Empty;
        return false;
    }
}
