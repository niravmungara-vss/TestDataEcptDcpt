using TestDataEcptDcpt.Attribute;

namespace TestDataEcptDcpt.Models
{
    public class UserModel
    {
        public string Name { get; set; }

        [EncryptionDecryptionAttr]
        public string Email { get; set; }
    }
}
