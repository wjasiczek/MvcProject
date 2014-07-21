using Moq;
using MvcProject.Controllers;
using System.Web.Mvc;
using Xunit;
using MvcProject.Models;
using System.Security.Principal;
using System.Web;
using System.ComponentModel;


namespace MvcProject.Tests.Controllers
{
    public class CrcControllerTest
    {
        [Fact]
        public void Should_return_original_model_if_model_isnt_valid()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            var controller = new CrcController();
            controller.ControllerContext = mockContext.Object;
            controller.ModelState.AddModelError("key", "errorMessage");

            var model = new CrcSubmitViewModel();          
            model.crcModel.binaryValue = "101101";
            model.crcModel.generator = "11";
            var initialModel = ReturnCopy(model);

            var result = controller.SubmitForm(model);

            Assert.Equal(model.crcModel.signal, initialModel.crcModel.signal);
        }

        [Fact]
        public void Should_return_updated_model_if_model_is_valid()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            var controller = new CrcController();
            controller.ControllerContext = mockContext.Object;

            var model = new CrcSubmitViewModel();
            model.crcModel.binaryValue = "101101";
            model.crcModel.generator = "11";
            var initialModel = ReturnCopy(model);

            var result = controller.SubmitForm(model);

            Assert.NotEqual(model.crcModel.signal, initialModel.crcModel.signal);
        }

        [Fact]
        public void Should_redirect_to_login_if_not_authenticated()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            var controller = new CrcController();
            controller.ControllerContext = mockContext.Object;

            var result = controller.CrcList(string.Empty) as RedirectToRouteResult;

            Assert.Equal("Account", result.RouteValues["controller"]);
            Assert.Equal("Login", result.RouteValues["action"]);
        }

        [Fact]
        public void Should_return_partial_view()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            var controller = new CrcController();
            controller.ControllerContext = mockContext.Object;

            var result = controller.CrcList("All") as PartialViewResult;

            Assert.Equal("_crcFilterList", result.ViewName);
        }

        [Fact]
        public void Should_return_true_with_given_values()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            var controller = new CrcController();
            controller.ControllerContext = mockContext.Object;

            var result = controller.CheckCorrectness("0000");

            Assert.Equal(true, result);
        }

        private CrcSubmitViewModel ReturnCopy(CrcSubmitViewModel modelToCopy)
        {
            var modelToReturn = new CrcSubmitViewModel();
            modelToReturn.crcModel = new CrcModel();

            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(modelToCopy.crcModel))
            {
                item.SetValue(modelToReturn.crcModel, item.GetValue(modelToCopy.crcModel));
            }

            return modelToReturn;
        }
    }
}
