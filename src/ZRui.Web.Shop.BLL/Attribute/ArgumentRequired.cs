using System.ComponentModel.DataAnnotations;

namespace ZRui.Web.BLL.Attribute
{
    public class ArgumentRequired: RequiredAttribute
    {
        public ArgumentRequired(string errorMessage):base()
        {
            ErrorMessage = errorMessage;
        }
    }
}
