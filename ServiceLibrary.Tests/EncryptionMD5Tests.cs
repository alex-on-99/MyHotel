using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceLibrary.Tests
{
    [TestClass]
    public class EncryptionMD5Tests
    {
        [TestMethod]
        public void Encrypt_inputString_encryptiondeStringReturned()
        {
            string input = "a1b2c3";
            string expected = "3c086f596b4aee58e1d71b3626fefc87";

            string actual = EncryptionMD5.Encript(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
