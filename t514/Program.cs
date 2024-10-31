using System.Text;
using t514_utils;

namespace t514_emulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string prog_code = File.ReadAllText("app.t514");
            Compiler compiler = new Compiler();
            int[] program = compiler.Compile(prog_code);
            Console.WriteLine(string.Join(", ", program));
            StringBuilder visibleapp = new StringBuilder();
            visibleapp.Append("0x");
            foreach (int i in program)
            {
                visibleapp.Append(i.ToString("X2"));
                visibleapp.Append(" ");
            }
            File.WriteAllText("app.t514o", visibleapp.ToString());
            if (0!=1)
            {
                Console.WriteLine("|||||||||");
                Emulator emu = new Emulator();
                emu.load_prog(program);
                emu.init();
                while (true)
                {
                    emu.tick();
                }
            }
        }
    }
}
