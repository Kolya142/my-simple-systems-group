
namespace t514_utils
{
    public class Compiler
    {
        private static Dictionary<string, int> _opcodes = new Dictionary<string, int>
        {
            {"mov", 0x00},
            {"movi", 0x01},
            {"movm", 0x02},
            {"movn", 0x03},
            {"movl", 0x04},
            {"xor", 0x05},
            {"or", 0x06},
            {"and", 0x07},
            {"add", 0x08},
            {"sub", 0x09},
            {"mul", 0x0a},
            {"div", 0x0b},
            {"mod", 0x0c},
            {"push", 0x0d},
            {"pop", 0x0e},
            {"jmp", 0x0f},
            {"call", 0x10},
            {"jl", 0x11},
            {"jb", 0x12},
            {"je", 0x13},
            {"jmpi", 0x14},
            {"calli", 0x15},
            {"jli", 0x16},
            {"jbi", 0x17},
            {"jei", 0x18},
            {"cmp", 0x19},
            {"rnt", 0x1a},
            {"int", 0x1b},
            {"ret", 0x1c}
        };
        private string[] _x16_regs = "ax, bx, cx, dx, ex, fx, gx, hx, ix, jx, kx, lx, mx, nx, ox, px, rx, sx, tx, ux, vx, wx, xx, yx, zx".Split(", ");
        private string[] _x8_regs = "ah, bh, ch, dh, eh, fh, gh, hh, ih, jh, kh, lh, mh, nh, oh, ph, rh, sh, th, uh, vh, wh, hh, yh, zh, al, bl, cl, dl, el, fl, gl, ll, il, jl, kl, ll, ml, nl, ol, pl, rl, sl, tl, ul, vl, wl, ll, yl, zl".Split(", ");
        public int parse_int_smart(string str)
        {
            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt32(str, 16);
            }
            else if (str.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt32(new string(str.Skip(2).ToArray()), 2);
            }
            else if (str.StartsWith("0o", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt32(new string(str.Skip(2).ToArray()), 8);
            }
            else
            {
                return int.Parse(str);
            }
        }
        public int[] Compile(string code)
        {
            code = code.ReplaceLineEndings("\n");
            List<string> regs = _x16_regs.Concat(_x8_regs).ToList();
            List<int> result = new List<int>();
            Dictionary<string, int> labels = new Dictionary<string, int>();
            string[] lines = code.Split("\n");
            int i = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith(";") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (line.StartsWith(":"))
                {
                    labels.Add(line.Substring(1, line.Length - 1), i);
                }
                i += 3;
            }
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string Line = line;
                if (line.StartsWith(":"))
                {
                    continue;
                }
                if (line.StartsWith(";"))
                {
                    continue;
                }
                foreach (string label in labels.Keys)
                {
                    Line = Line.Replace(label, labels[label].ToString());
                    Console.WriteLine($"{label} -> {labels[label]}");
                }
                string[] sp = Line.Split(" ");
                string n = new string(Line.Skip(Line.IndexOf(' ') + 1).ToArray()).Replace(" ", "");
                string[] sp1 = n.Split(",");
                int opcode = _opcodes[sp[0]];
                int r1 = 0, r2 = 0;
                if (sp1.Length == 0 || sp1[0] == "")
                {
                    // do nothing
                }
                else if (sp1.Length == 1 || sp1[1] == "")
                {
                    if (regs.Contains(sp1[0]))
                    {
                        r1 = regs.IndexOf(sp1[0]);
                    }
                    else
                    {
                        r1 = parse_int_smart(sp1[0]);
                    }
                }
                else
                {
                    if (regs.Contains(sp1[0]))
                    {
                        r1 = regs.IndexOf(sp1[0]);
                    }
                    else
                    {
                        r1 = parse_int_smart(sp1[0]);
                    }
                    if (regs.Contains(sp1[1]))
                    {
                        r2 = regs.IndexOf(sp1[1]);
                    }
                    else
                    {
                        r2 = parse_int_smart(sp1[1]);
                    }
                }
                result.Add(opcode);
                result.Add(r1);
                result.Add(r2);
                i += 3;
            }
            return result.ToArray();
        }
    }
}
