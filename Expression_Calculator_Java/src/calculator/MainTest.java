package calculator;

class MainTest {

    public static void test() {

        System.out.println("starting test ...");

        CalculatorTest test = new CalculatorTest();
        test.Error();
        test.isNullOrEmpty();
        test.numbers();
        test.validate();
        test.prepare();
        test.postFix();
        test.resolve();
        test.calc();
        test.extra();

        System.out.println("completed test.");
    }
}