﻿namespace Moqesq.Tests
{
    public class SomeClass
    {
        ITest test;

        public SomeClass(ITest test)
        {
            this.test = test;
        }

        public void Bla()
        {
            test.DoTestThing();
        }
    }
}
