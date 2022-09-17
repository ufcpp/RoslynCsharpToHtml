using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;

namespace CsharpToHtml;

public readonly record struct Tag(int Position, int Ordinal, string? ClassName = null, string? DiagnosticId = null) : IComparable<Tag>
{
    public static Builder GetBuilder() => new();

    public bool IsClose => ClassName is null;

    public int CompareTo(Tag other)
    {
        var pos = Position.CompareTo(other.Position);
        if (pos != 0) return pos;

        // close tags must preceeds open tags.
        if (IsClose && !other.IsClose) return -1;
        if (!IsClose && other.IsClose) return 1;

        var ord = Ordinal.CompareTo(other.Ordinal);

        if (IsClose) ord = -ord; // reverse order for close tags.

        return ord;
    }

    public struct Builder
    {
        private int _ordinal = 0;
        private readonly List<Tag> _tags = new();
        public Builder() { }

        public Builder Append(ClassifiedSpan span)
        {
            var @class = ClassTable.TypeToClass(span.ClassificationType);

            if (@class is null) return this;

            var ord = _ordinal++;
            _tags.Add(new(span.TextSpan.Start, ord, @class));
            _tags.Add(new(span.TextSpan.End, ord));

            return this;
        }

        public Builder Append(IEnumerable<ClassifiedSpan> classifiedSpans)
        {
            foreach (var span in classifiedSpans)
                Append(span);

            return this;
        }

        public Builder Append(Diagnostic diag)
        {
            var sevirity = diag.Severity switch
            {
                DiagnosticSeverity.Warning => "warning",
                DiagnosticSeverity.Error => "error",
                _ => null,
            };
            if (sevirity is null) return this;

            var ord = _ordinal++;
            _tags.Add(new(diag.Location.SourceSpan.Start, ord, sevirity, diag.Id));
            _tags.Add(new(diag.Location.SourceSpan.End, ord));

            return this;
        }

        public Builder Append(IEnumerable<Diagnostic> diags)
        {
            foreach (var diag in diags)
                Append(diag);

            return this;
        }

        public Queue Build()
        {
            _tags.Sort();
            return new(_tags);
        }
    }

    public struct Queue
    {
        private Queue<Tag> _queue;
        internal Queue(IEnumerable<Tag> items) => _queue = new(items);

        public bool TryGet(int position, out Tag tag)
        {
            if (_queue.Count == 0)
            {
                tag = default;
                return false;
            }

            tag = _queue.Peek();
            if (tag.Position == position)
            {
                _queue.Dequeue();
                return true;
            }
            return false;
        }
    }
}
