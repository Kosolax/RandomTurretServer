using System;
using System.Collections.Generic;

public class ErrorException : Exception
{
    public ErrorException() : base()
    {
    }

    public ErrorException(string message) : base(message)
    {
    }

    public ErrorException(string message, Exception inner) : base(message, inner)
    {
    }

    public ErrorException(Dictionary<string, string> errors)
    {
        this.Errors = errors;
    }

    public Dictionary<string, string> Errors { get; set; }
}