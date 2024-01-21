using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSMB_RNG_WebApp;

namespace TEST_NSMB_RNG
{
	[TestClass]
	public class TestSeedController
	{
		[TestMethod]
		public void RespondsToBadRequest()
		{
			var controller = new NSMB_RNG_WebApp.Controllers.SeedsController();
			var result = controller.Seeds("xxxxxxx");
			Assert.AreEqual(result.Value, null);
		}
	}
}