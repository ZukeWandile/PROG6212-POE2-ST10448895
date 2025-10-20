using PROG6212_POE2.Helpers;
using ST10448895_CMCS_PROG.Helpers;
using Xunit;

namespace PROG6212_POE2.Tests
{
    public class EncryptionHelperTests
    {
        [Fact]
        public void Encrypt_Then_Decrypt_ReturnsOriginalText()
        {
            string input = "Hello123!";
            string encrypted = EncryptionHelper.Encrypt(input);
            string decrypted = EncryptionHelper.Decrypt(encrypted);

            Assert.Equal(input, decrypted);
        }

        [Fact]
        public void Encrypt_DifferentInputs_ProduceDifferentResults()
        {
            string input1 = "A";
            string input2 = "B";

            string encrypted1 = EncryptionHelper.Encrypt(input1);
            string encrypted2 = EncryptionHelper.Encrypt(input2);

            Assert.NotEqual(encrypted1, encrypted2);
        }
    }
}
