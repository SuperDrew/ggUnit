namespace GgUnit.CollectiveCheck
{
    using System;
    using System.Collections;

    using Generator;

    public static class Check
    {
        public delegate void TestDelegate();

        public static Result AreEqual(string actual, string expected, string name, string detail = null)
        {
            if (actual == expected)
            {
                detail = detail
                         ?? string.Format(
                             "Value was '{0}'",
                             StringUtils.ReplaceNullOrEmptyStringWithReadableValues(actual));
                return new Result(State.Passed, name, detail);
            }

            detail = detail
                     ?? string.Format(
                         "Value should be '{0}', but was '{1}'",
                         StringUtils.ReplaceNullOrEmptyStringWithReadableValues(expected),
                         StringUtils.ReplaceNullOrEmptyStringWithReadableValues(actual));
            return new Result(State.Failed, name, detail);
        }

        public static Result AreEqual<T>(T actual, T expected, string name, string detail = null) where T : struct
        {
            if (actual.Equals(expected))
            {
                detail = detail ?? string.Format("Value was '{0}'", actual);
                return new Result(State.Passed, name, detail);
            }

            detail = detail
                     ?? string.Format(
                         "Value should be '{0}', but was '{1}'", expected, actual);
            return new Result(State.Failed, name, detail);
        }

        public static Result DoesNotThrow(TestDelegate testDelegate, string name)
        {
            try
            {
                testDelegate.Invoke();
            }
            catch (Exception ex)
            {
                return new Result(State.Failed, name, "Threw" + ex);
            }

            return new Result(State.Passed, name, "Did not throw");
        }

        public static Result HasCountGreaterThan(ICollection enumerable, int count, string name, string detail = null)
        {
            var actualCount = enumerable.Count;
            if (actualCount > count)
            {
                detail = detail ?? string.Format("Count should be > {0}, was {1}", count, actualCount);
                return new Result(State.Passed, name, detail);
            }

            detail = detail
                     ?? string.Format(
                         "Count should have been > {0}, but was {1}", count, actualCount);
            return new Result(State.Failed, name, detail);
        }

        public static class Is
        {
            public static Result NotNull(object value, string name, string detail = null)
            {
                if (value != null)
                {
                    detail = detail ?? "Value should not be null and wasn\'t null.";
                    return new Result(State.Passed, name, detail);
                }

                detail = detail ?? "Value should not be null but was null.";
                return new Result(State.Failed, name, detail);
            }

            public static Result Null(object value, string name, string detail = null)
            {
                if (value == null)
                {
                    detail = detail ?? "Value should be null and was null.";
                    return new Result(State.Passed, name, detail);
                }

                detail = detail ?? "Value should not be null and wasn\'t null.";
                return new Result(State.Failed, name, detail);
            }

            public static Result StringContaining(string message, string contains, string name, string detail = null)
            {
                if (message == null)
                {
                    detail = detail ?? string.Format("String should have contained '{0}', but was <null>", contains);
                    return new Result(State.Failed, name, detail);
                }

                if (message.Contains(contains))
                {
                    detail = detail ?? string.Format("String contained '{0}', was '{1}'", contains, message);
                    return new Result(State.Passed, name, detail);
                }

                detail = detail ?? string.Format("String should have contained '{0}', but was '{1}'", contains, message);
                return new Result(State.Failed, name, detail);
            }
        }
    }
}