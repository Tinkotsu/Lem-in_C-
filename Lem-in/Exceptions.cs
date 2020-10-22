using System;

namespace Lem_in
{
    public class Exceptions
    {
        public class ArgumentsException: Exception
        {
            public ArgumentsException()
                : base("Wrong arguments!")
            { }
        }

        public class MapException: Exception
        {
            public MapException(string message)
                : base($"Map is not valid! {message}")
            { }
        }
        public class NoPathException : Exception
        {
            public NoPathException()
                : base("No Path!")
            { }
        }
    }
}