namespace t514_emulator
{
    /*
    0x00 -- mov reg1, reg2
    0x01 -- movi reg, value
    0x02 -- movm address, reg
    0x03 -- movn reg, address
    0x04 -- movl address, value
    0x05 -- xor reg1, reg2
    0x06 -- or reg1, reg2
    0x07 -- and reg1, reg2
    0x08 -- add reg1, reg2
    0x09 -- sub reg1, reg2
    0x0a -- mul reg1, reg2
    0x0b -- div reg1, reg2
    0x0c -- mod reg1, reg2
    0x0d -- push reg
    0x0e -- pop reg
    0x0f -- jmp address
    0x10 -- call address
    0x11 -- jl address -- is lower
    0x12 -- jb address -- is bigger
    0x13 -- je address -- is equ
    0x14 -- jmpi reg
    0x15 -- calli reg
    0x16 -- jli reg
    0x17 -- jbi reg
    0x18 -- jei reg
    0x19 -- cmp r1, r2
    0x1a -- rnt address -- register int (ax - id)
    0x1b -- int id
    0x1c -- ret
     */
    public class Emulator
    {
        int[] regs = new int[78]; // TODO: Implement 8 bit and 16 bit registers
        int[] memory = new int[0xfffff]; // 1MB memory
        Dictionary<int, int> interupts = new Dictionary<int, int>();
        /*
        stack reg - 0x18 zx
        program address - 0xE ox

        vga - 0x100 - 0x8d0
        stack - 0x9d0 - 0x109cf
        program - 0x109cf - 0x10fcf
        free memory - 0x10fcf - 0xfffff
        */
        public void load_prog(int[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                memory[i+0x109cf] = program[i];
            }
        }
        public void init()
        {
            regs[0xE] = 0x109cf;
            regs[0x18] = 0x9d0;
        }
        public void tick() 
        {
            int i = regs[0xE];
            int opcode = memory[i];
            int r1 = memory[i+1];
            int r2 = memory[i+2];
            switch (opcode)
            {
                case 0x00:
                    regs[r1] = regs[r2];
                    break;
                case 0x01:
                    regs[r1] = r2;
                    break;
                case 0x02:
                    memory[regs[r1]] = regs[r2];
                    break;
                case 0x03:
                    regs[r1] = memory[r2];
                    break;
                case 0x04:
                    memory[r1] = r2;
                    break;
                case 0x05:
                    regs[r1] ^= regs[r2];
                    break;
                case 0x06:
                    regs[r1] |= regs[r2];
                    break;
                case 0x07:
                    regs[r1] &= regs[r2];
                    break;
                case 0x08:
                    regs[r1] += regs[r2];
                    break;
                case 0x09:
                    regs[r1] -= regs[r2];
                    break;
                case 0x0a:
                    regs[r1] *= regs[r2];
                    break;
                case 0x0b:
                    regs[r1] /= regs[r2];
                    break;
                case 0x0c:
                    regs[r1] %= regs[r2];
                    break;
                case 0x0d:
                    memory[regs[0x18]] = regs[r1];
                    regs[0x18]++;
                    break;
                case 0x0e:
                    regs[0x18]--;
                    regs[r1] = memory[regs[0x18]];
                    break;
                case 0x0f: // jmp
                    regs[0xE] = r1;
                    break;
                case 0x10: // call
                    memory[regs[0x18]] = regs[0xE]; // push
                    regs[0x18]++;

                    regs[0xE] = r1; // jmp
                    break;
                case 0x11: // jl
                    regs[0x18]--;
                    int m = memory[regs[0x18]];
                    if ((m & 0x1) != 0)
                    {
                        regs[0xE] = r1;
                    }
                    break;
                case 0x12: // jb
                    regs[0x18]--;
                    int m3 = memory[regs[0x18]];
                    if ((m3 & 0x10) != 0)
                    {
                        regs[0xE] = r1;
                    }
                    break;
                case 0x13: // je
                    regs[0x18]--;
                    int m4 = memory[regs[0x18]];
                    if ((m4 & 0x100) != 0)
                    {
                        regs[0xE] = r1;
                    }
                    break;
                case 0x14: // jmpi
                    regs[0xE] = regs[r1];
                    break;
                case 0x15: // calli
                    memory[regs[0x18]] = regs[0xE]; // push
                    regs[0x18]++;

                    regs[0xE] = regs[r1]; // jmp
                    break;
                case 0x16: // jli
                    regs[0x18]--;
                    int mm = memory[regs[0x18]];
                    if ((mm & 0x1) != 0)
                    {
                        regs[0xE] = regs[r1];
                    }
                    break;
                case 0x17: // jbi
                    regs[0x18]--;
                    int m3m = memory[regs[0x18]];
                    if ((m3m & 0x10) != 0)
                    {
                        regs[0xE] = regs[r1];
                    }
                    break;
                case 0x18: // jei
                    regs[0x18]--;
                    int m4m = memory[regs[0x18]];
                    if ((m4m & 0x100) != 0)
                    {
                        regs[0xE] = regs[r1];
                    }
                    break;
                case 0x19:
                    int m1 = regs[r1];
                    int m2 = regs[r2];
                    int flags = 0;
                    if (m1 < m2)
                        flags |= 0b1;
                    if (m1 > m2)
                        flags |= 0b10;
                    if (m1 == m2)
                        flags |= 0b100;

                    memory[regs[0x18]] = flags; // push
                    regs[0x18]++;
                    break;
                case 0x1a: // rnt
                    int id = regs[0];
                    interupts.Add(id, r1);
                    break;
                case 0x1b: // int
                    memory[regs[0x18]] = regs[0xE]; // push
                    regs[0x18]++;

                    regs[0xE] = interupts[regs[0]]; // jmp
                    break;
                case 0x1c: // ret
                    regs[0x18]--;
                    regs[0xE] = memory[regs[0x18]];
                    break;
            }
            /*
            0x11 -- jl address -- is lower                [*]
            0x12 -- jb address -- is bigger               [*]
            0x13 -- je address -- is equ                  [*]
            0x14 -- jmpi reg                              [*]
            0x15 -- calli reg                             [*]
            0x16 -- jli reg                               [*]
            0x17 -- jbi reg                               [*]
            0x18 -- jei reg                               [*]
            0x19 -- cmp r1, r2                            [*]
            0x1a -- rnt address -- register int (ax - id) [*]
            0x1b -- int id                                [*]
            0x1c -- ret                                   [*]
            */
            if (regs[0xE] == i) {
                regs[0xE] += 3;
            }
            vga_update();
        }
        public void vga_update()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);
            for (int i = 0x100; i < 0x8d0; i++)
            {
                if (memory[i] == 0)
                    Console.Write(" ");
                else
                    Console.Write((char)memory[i]);
            }
            Console.SetCursorPosition(0, 24);
            Console.Write(regs[0x0].ToString("X2"));
        }
    }
}
