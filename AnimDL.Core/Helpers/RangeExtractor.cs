﻿namespace AnimDL.Core.Helpers
{
    public static class RangeExtractor
    {
        public static (int Start, int End) Extract(this Range range, int count)
        {

            int start = range.Start.IsFromEnd ? count - range.Start.Value + 1 : range.Start.Value;
            int end;

            if(range.End.IsFromEnd)
            {
                if(range.End.Value == 0)
                {
                    end = count;
                }
                else
                {
                    end = count - range.End.Value + 1;
                }
            }
            else
            {
                end = range.End.Value;
            }

            return (start, end);
        }
    }
}
