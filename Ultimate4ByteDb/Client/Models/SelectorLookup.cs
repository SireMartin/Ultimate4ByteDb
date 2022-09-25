using Blazored.FluentValidation;
using FluentValidation;

namespace Ultimate4ByteDb.Client.Models
{
    public class SelectorLookup
    {
        public string Selector { get; set; }
    }

    public class SelectorLookupValidatior : AbstractValidator<SelectorLookup>
    {
        public SelectorLookupValidatior() 
        {
            RuleFor(x => x.Selector).Must(x => x.Length == 8 || x.Length == 10).WithMessage("Selector must have length 8 (without 0x-prefix) or 10 (with 0x-prefix)");
            RuleFor(x => x.Selector).Matches(@"^(0x)?[0-9a-fA-F]+$").WithMessage("Selector may only contian hexadecimal characters, optionally preceded with 0x");
        }
    }
}
