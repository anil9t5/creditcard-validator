using Xunit;

public class TestClass
{

    [Fact]
    public void PassingCreditCardTest()
    {

        Assert.Equal(true, mfiles.Program.ValidateCardNumber("4485958989016410"));
        Assert.NotEqual(true, mfiles.Program.ValidateCardNumber("4485958989016411"));
        Assert.Equal(true, mfiles.Program.ValidateCardNumber("5301 8656 4887 7620"));
    }
}