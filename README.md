
Summary
============

Testing library providing a fluent tdd/bdd testing approach. Can be easily integrated 
into your own testing tools (nunit, xunit, moq, etc)

The goals behind this library are:

Comprehension
------------

* tests should be easy to understand. Since code is read far more than written, focus on the read part.
* keep tests as close to natural language as possible, though try to eliminate ambiguity
* if possible also make tests easy to write. This results in more test code and more fun

Making the IDE work for you
------------
   
* support autocomplete.
* support refactoring.
* make it easy to jump directly to code doing the actual work.

Integration
------------

* easily integrate with existing test infrastructure

Extensibility
------------
        
* make it easy to customise your tests.
* infrastructure should aid not hinder.
* allow you to do your own thing when required. Replace the bits you need without having to reimplement all the parts
* not force you to do it a certain way, though encouraging conventions helps.

Discoverability
------------

* discovery of things should be easy without the need to go and read the documentation. 
* a consistent and easy to guess naming pattern.

Diagnostics
------------

If a test fails tell explain why and provide as much info as possible why it failed. Don't tell me foo is wrong, 
tell me why and what was expected and what was actually found, and any other related info.

DRY
------------

Don't repeat the same boring string assertion, or complex relationship tests across all your tests. Put it 
in a single place (your Matcher).

Example
------------

The scenario:

    "Given a registered user, with an account balance of 10 dollars, when 6 dollars is debited, then 
    expect that the account only has 4 dollars, and the transaction ledger records a 6 dollar debit"

can be written in TestFirst.Net as:

    Scenario()
        .Given(user = UserInTheDb().WithDefaults())
        .Given(account = AccountInTheDb().For(user).Balance(10).Dollars())
        
        .When(()=>account.debit(6))

        .Then(
            ExpectThat(TheAccountInTheDb.For(account)),
            Is(AnAcccount.With().Balance(4).Dollars())
        .Then(
            ExpectThat(TheAccountLedgerInTheDb.For(account)),
            Is(AnLedger.With().Debit(6).Dollars());
    
Which depending on the number of beers you've drunk resembles the natural english version fairly closely without the need of an additional
template translation step in the process (so refactoring won't kill things, find references still work, can step directly to the assertion code)    

Matchers
============

TestFirst.Net centers around the concept of a 'Matcher'. This is a class separate from your individual unit test to perform assertions
on the objects and services under test. Naming convention is that matchers names start with an 'A' or an 'An'

Examples:

    AString.Contains(...)
    AnInt.Between(2,5)
    ADateTime.Now().Within(3).Seconds()
    ACookie.With().Name("foo").Value(AString.MatchingAndPattern("?bar*)).Expires(ADateTime.After(DateTime.Now)));
    AFoo.With().Bar("123").Fibble(AFibble.With().BigEars().NoMoustache());

The idea is that as you flesh out your tests over time, more and more of the test code can be reused. This is
versus all the assertions repeated in your test. So for example if there is a concept of a 'valid' email address, 
there can be an email address matcher, or in combination with your user matcher you can call:

    AUser.With().ValidEmail() under the hood might call 'AnEmailAddress.IsValid(...)'

Ditto for things like a valid user account, trade, registration etc

As more and more matchers are added over time, more of your tests can be written using existing matchers. If some
concept changes, in theory only a few locations in your test code need to also.

Remember, auto complete is your friend! If you stick to the naming conventions, you only need to start typing 
and in theory completions should come up. 

The static create method is usually 'With' for complex matchers, or simply the matcher method for simpler 
ones. Sometimes 'Of' or 'For' are used if this aids in the fluent langauge

There are basic matchers for most of the primitive types, along for other base types. Simply hit 'A' and start typing. 
Some existing ones are 'AString', 'AnInt', 'ADecimal', 'ADateTime', 'AList','ADictionary', 'AnInstance'. Others are 
for you to write for your own objects. The neat thing is this can then be reused across tests. 

There is a code generator to generate matchers automatically for each of your objects.

Usage
============

Using just the assertions
----------

This is useful if you only want to make use of the matchers and/or sprinkle your existing tests with a bit of TestFirst.Net

Examples:

    ...do stuff in your test

    Expect
        .That(foolist) <--thing we want to assert
        .Is(AList.InAnyOrder().WithOnly("a").And("b")); <--'Matcher' we are using to perform the 

    Expect
        .That(()=>foo.DoIt())
        .Throws(AnException.Of().Type<MyException>().Message(AString.ContainingIgnorePunctuationAndCase("Some bad thing")));
    
    Expect
        .That(()=>foo.map())
        .Is(ADictionary
            .KeyMatching("a","value_a")
            .KeyMatching("foo",AFoo.With().FloppyEars().SunHat()));

Scenario based
----------
Setup scenarios which provide injection, db creation, db insertion and retrieval, service startup, resource cleanup. 

You can still roll it as you wish by combining or swapping out the parts yourself. This still integrates with your existing test classes.

If extending fo NUnit or Moq, include nuget package TestFirst.Net.Extensions

    [TestFixture]
    MyTestScenario : AbstractNUnitScenarioTest {...//or AbstractNUnitMoqScenarioTest or ScenarioFluency

        [Test]
        public void HappyPathUserCorrectlyRegistered(){
            RegistrationService service;
            Registration reg;            

            Scenario() <-- passes back a 'Scenario' object with an injector
                .Given(reg = UserRegistration.With().Defaults().Age(123).RandomPassword()..)
                .Given(service = NewRegistrationService())    <--this is disposed at end of test
                .When(service.register(reg))//<!--interesting part of test
                .Then(
                    ExpectThat(UserInTheDb.With().UserName(reg.UserName)), <-- UserInTheDb is a 'Fetcher'
                    Is(AUserInTheDb.With().Age(reg.Age)...))
                .Then(
                    ExpectThat(LoginInTheDb.With().UserName(reg.UserName)),
                    Is(ALoginInTheDb.With().Password(reg.Password)...))
                .Then(
                    ExpectThat(AccountInTheDb.With().UserName(reg.UserName)),
                    Is(AnAccount.With().Created(ADateTime.After(now)...))

        }
    }

Test now clearly shows the setup, the operation(s) we're testing, and the expected result

The 'XInTheDb', as in 'UserInTheDb' implements the 'IFetcher' interface. This simply is used to look things up (how you do it 
up to you). It will also have dependencies injected. Up to you to write or generate this class.

The 'Given', 'When', 'Then' methods are chained builder methods taking in a number of different parameter types. All objects
passed in have their dependencies set via the IStepArgDependencyInjector. The default setup simply
collects all objects implementing 'IDisposable' and disposes of them in reverse order on test method exit.

To customise the scenario, call the various 'UseX...' methods before creating the scenario, as in:

    UseScenarioInjector(myInjector);

    Scenario()
        ...

For custom injection for all tests simply create your own base test case which sets the various options in the call to 'Scenario',
or implement 'OnBeforeNewScenario'    

Constituent Parts
============

Matchers (IMatcher)
----------

As mentioned previously, these are used to match results against

Fetchers  (IFetcher)
----------

These fetch things given a set of criteria. They can be passed to a 'Then' method to find the object to assert on using 
a given matcher. Simply implement a 'Fetch' method.

Inserters (IInserter)
----------

These insert things. What they insert into what is up to you. They simply implement an 'Insert' method. Disabled if
running tests against a prod database

Updaters (IUpdater)
----------

Perform cleanup,delete or update stuff. Implements, you guessed it, 'Update'.

Invokers (IInvokable)
----------

Do something when passed to 'Given','When','Then'. Implements 'Invoke'

Builders (IBuilder)
----------

Anything passed to 'Given','When','Then' which is a builder will have it's 'Build' method called. This allows for building
up complex test state in an easy to read way. Builders also have depedencies injected.
    
Dependency injector (IStepArgDependencyInjector)
----------

All the objects passed to 'Given', 'When', 'Then' are passed through this injector to have dependencies set. 

Utilities
============


TestFirst.Net.Rand.Random
----------

A more useful random value generator.

Example:

    var rand = new Random();
    foo.setName(rand.AlphaNumericString(7)).setMyEnum(rand.EnumOf<MyEnum>());
        

TestFirst.Net.Rand.RandomFiller
----------    

To fill poco's with random data.

Example:

    var filler = new RandomFiller.With()
        .GeneratorForType(typeof(Fibble), () => Fibble.CreateRandom()) //customise random value generators
        .EnableLogging(true)
        .Build();

    var poco = filler.FillWithRandom(new MyPoco());//do the actual filling

Moq Scenario
============

Include nuget package TestFirst.Net.Extensions

Extend AbstractNUnitMoqScenarioTest to provide easy creation and registration of Moq mocks.

If using MatcherMoqExtensions, a fullish example would be:

[TestFixture]
MyTestClass : AbstractNUnitMoqScenarioTest {

    [Test]
    public void myTest(){
     
        MyClass foo;
        String response;

        Scenario() //this will set the scenario name to the name of your test method, in this case 'myTest'
             .Given(foo=AMock<MyClass>() //setup the mock
                     .WhereMethod(f=>f.DoIt(
                         ArgIs(AString.EndingWith("It")),
                         ArgIs(AnInt.GreaterThan(0))
                     )
                     .Returns("done!")
                     .WhereMethod(f=>f.DoneIt(
                         ArgIs(AString.EqualTo("done!"))
                     )
                     .Instance //returns the mock object and assigns to foo
             )
         .When(response=foo.DoIt("WorkIt",2)) //invoke the 1st method
         .Then(ExpectThat(response),Is(AString.EqualTo("done!"))
         .When(foo.DoneIt(response)) //invoke the 2nd method
         .Then(Nothing())//you shoud assert something here

        //Moq's Mock.VerifyAll will be automatically called at the end of the scenario
    }
}

Performance testing
============

Include nuget package TestFirst.Net.Performance

Example:

    [TestFixture]
    public class MyPerfTest:AbstractNUnitScenarioTest
    {
        [Test]        
        public void WhateverPerfTest()
        {
            PerformanceMetricsWriter metricsWriter;
            PerformanceReport report;

            Scenario()
                .Given(metricsWriter = PerformanceMetricsWriter.With().TestName("WhateverPerfTest"))
                .When(PerformanceSuite.With()
                    .NumRuns(2)
                    .PerRunTimeout(20).Seconds()
                    .LoadRunner(ContentionLoadRunner.With()
                        .Tests(new MyPerfTest()) //the test to run
                        .RunTimeout(15).Seconds())
                    .Listener(metricsWriter)
                    .Build())//runs the test (returns an IInvokable)
                .When(report = metricsWriter.BuildReport())//no magic, just assignment
                .When(report.PrintToConsole)//just an action, nothing magic
                .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMean),
                    Is(ADouble.EqualTo(4.75)))
                .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMedian),
                    Is(ADouble.EqualTo(5)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMax),
                    Is(ADouble.EqualTo(10)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMin),
                    Is(ADouble.EqualTo(0)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").MetricName),
                    Is(AString.EqualTo("metric1")));
        }

    ...
    }   

Any part of the above can be replaced with your own implementation if you don't like how the provided classes work.

Output report will look something like:

    Generated At: XXXX
         Metrics: "<path-to-results>.csv"
         Summary:
    For metric 'metric1'
            Total Metric Count: 10
                      Ok Count: 8
                   Error Count: 2 (ignored in calculations)
             Throughput/Second: 8
                  Metric Units: ms
                       Min Val: 0
                      Mean Val: 4.75
                    Median Val: 5
                       Max Val: 10
                  StdDeviation: 3.56195171219375
        StdDeviation % of Mean: 74.9884570988158%
       68% confidence interval (+/-1 std deviation)
                              : 1.18804828780625 to 8.31195171219375
       95% confidence interval (+/-2 std deviations)
                              : -2.3739034243875 to 11.8739034243875
    For metric 'metric2'
        .....


Automatic Matcher code generation
============

In a T4 template, use the following:


    var template = new MatchersTemplate();

    //customise generation
    template.ForPropertyType<String>()
        .AddMatchMethodTaking<int>("$argName.ToString()");<--for any String property, add a match method taking an int
    
    //genrate matchers for the following
    template.GenerateFor<MyPoco>();
    template.GenerateFor<MyPoco2>().MatcherName("MyFibble");//customise matcher name
    template.GenerateFor<MyPoco3>().MatcherName("MyFrubble").ExcludeProperties("MyExcludedProp");//further customisation
    
    template.RenderToFile("MyMatchers.cs");<--will write all the generated matchers to the given file
    
Building
=============

This library is built and tested on mono. It uses Nuget and xbuild. It should 'just' build in Visual Studio.

To build and test from  the command line (bash), run the build script:

    ./build clean build test

For further options run:

    ./build help


