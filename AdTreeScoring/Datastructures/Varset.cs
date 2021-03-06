﻿using System.Collections;
using System.Collections.Generic;
using System;

namespace Datastructures
{
    class Varset
    {
        public Varset(int size)
        {
            item = new BitArray(size);
        }

        public Varset(Varset varset)
        {
            item = new BitArray(varset.item);
        }

        public Varset(ulong value)
        {
            int index = 0;
            item.Length = 64;
            for (ulong n = value; n > 0;)
            {
                ulong r = n % 2;
                if (r == 1)
                {
                    item.Set(index, true);
                }
                else
                {
                    item.Set(index, false);

                }
                n = n / 2;
                index += 1;
                item.Length += 1;
            }
            item.Length -= 1;
        }

        public bool Equals(Varset varset)
        {
            Varset cp = new Varset(this);
            Varset cp2 = new Varset(varset);
            cp.AlignLength(cp2);

            for (int i = 0; i < cp.item.Length; i++)
            {
                if (cp.item[i] != cp2.item[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool LessThan(Varset varset)
        {
            Varset cp = new Varset(this);
            Varset cp2 = new Varset(varset);
            cp.AlignLength(cp2);

            for (int i = cp.item.Length - 1; i >= 0; i--)
            {
                if (cp.item[i] && !cp2.item[i])
                {
                    return false;
                }
                else if (!cp.item[i] && cp2.item[i])
                {
                    return true;
                }
            }

            return false;
        }

        public void Set(int index, bool value)
        {
            if (index >= item.Count)
            {
                item.Length = index + 1;
            }
            item.Set(index, value);
        }

        public void SetAll(bool value)
        {
            item.SetAll(value);
        }

        public void SetFromCsv(string csv)
        {
            // do nothing for empty strings
            if (csv.Length == 0)
            {
                return;
            }

            // split out the scc variables
            List<string> tokens = new List<string>();
            string[] delimiters = { "," };
            tokens.AddRange(csv.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

            for (int i = 0; i < tokens.Count; i++)
            {
                int var = int.Parse(tokens[i]);

                // at least a little error checking
                if (var == 0 && tokens[i] != "")
                {
                    throw new FormatException("Invalid csv string: '" + csv + "'");
                }
                Set(var, true);
            }
        }

        public bool Get(int index)
        {
            if (index >= item.Count)
            {
                return false;
            }
            return item.Get(index);
        }

        public int FindFirst()
        {
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindNext(int index)
        {
            Varset tmp = new Varset(this);
            tmp = tmp.RightShift(index);
            return tmp.FindFirst() + index + 1;
        }

        public void Print()
        {
            string s = "{";
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i])
                {
                    s += i + ", ";
                }
            }
            s += "}";
            Console.WriteLine(s);
        }

        public static Varset ClearCopy(Varset varset, int index)
        {
            Varset cp = new Varset(varset);
            cp.Set(index, false);
            return cp;
        }

        public Varset LeftShift(int i)
        {
            Varset cp = new Varset(this);
            cp.item.Length += i;
            for (int k = cp.item.Length - 1; k > 0; k--)
            {
                cp.item[k] = cp.item[k - i];
            }
            cp.item[0] = false;
            return cp;
        }

        public Varset RightShift(int i)
        {
            Varset cp = new Varset(this);
            for (int k = 0; k < cp.item.Count - i; k++)
            {
                cp.item[k] = cp.item[k + i];
            }

            for (int k = cp.item.Count - i; k < cp.item.Count; k++)
            {
                cp.item[k] = false;
            }
            return cp;
        }

        public Varset And(Varset varset)
        {
            Varset cp = new Varset(this);
            cp.AlignLength(varset);
            cp.item.And(varset.item);
            return cp;
        }

        public Varset Or(Varset varset)
        {
            Varset cp = new Varset(this);
            cp.AlignLength(varset);
            cp.item.Or(varset.item);
            return cp;
        }

        public Varset Xor(Varset varset)
        {
            Varset cp = new Varset(this);
            cp.AlignLength(varset);
            cp.item.Xor(varset.item);
            return cp;
        }

        public Varset Not()
        {
            Varset cp = new Varset(this);
            cp.item.Not();
            return cp;
        }

        public Varset Add(Varset varset)
        {
            Varset cp = new Varset(this);
            Varset zero = new Varset(0);
            cp.AlignLength(varset);
            while (!varset.Equals(zero))
            {
                Varset tmp = cp.And(varset).LeftShift(1);
                cp = cp.Xor(varset);
                varset = tmp;
            }
            return cp;
        }

        public Varset StaticAdd(Varset varset)
        {
            Varset cp = new Varset(this);
            int length = cp.item.Count;
            cp = cp.Add(varset);
            cp = cp.SubVarset(length);
            return cp;
        }

        private Varset SubVarset(int index)
        {
            Varset cp = new Varset(this);
            //if (index < cp.item.Count)
            //{
            //    for (int i = index; i < cp.item.Count; i++)
            //    {
            //        cp.item[i] = false;
            //    }
            //}
            cp.item.Length = index;
            return cp;
        }

        public Varset Subtract(Varset varset)
        {
            Varset cp = new Varset(varset);
            if (cp.item.Length < item.Length)
            {
                AlignLength(cp);
            }
            Varset one = new Varset(1);
            one.Set(0, true);
            cp = cp.Not();
            cp = cp.StaticAdd(one);
            cp = cp.StaticAdd(this);
            return cp;
        }

        public Varset Divide(Varset varset)
        {
            int length = this.item.Count;
            Varset n = new Varset(this);
            Varset d = new Varset(varset);
            //Console.Write("n: ");
            //n.Print();
            //Console.Write("d: ");
            //d.Print();
            Varset m = new Varset(n.item.Length);
            m.Set(0, true);
            Varset q = new Varset(n.item.Count);
            Varset zero = new Varset(n.item.Count);

            if (n.Equals(zero))
            {
                return zero;
            }
            else if (d.Equals(zero))
            {
                throw new ArgumentException("Zero Division.");
            }

            while (d.LessThan(n) || d.Equals(n))
            {
                d = d.LeftShift(1);
                m = m.LeftShift(1);
            }
            Varset one = new Varset(n.item.Length);
            one.Set(0, true);
            while (one.LessThan(m))
            {
                d = d.RightShift(1);
                m = m.RightShift(1);
                if (d.LessThan(n) || d.Equals(n))
                {
                    n = n.Subtract(d);
                    q = q.Or(m);
                }
            }
            q = q.SubVarset(length);
            n = n.SubVarset(length);
            //Console.Write("q: ");
            //q.Print();
            //Console.Write("n: ");
            //n.Print();
            return q;
        }

        public Varset NextPermutation()
        {
            Varset vs = new Varset(this);
            Varset one = new Varset(0);
            one.Set(0, true);
            Varset tmp = vs.Or(vs.Subtract(one)).StaticAdd(one);
            Varset nextVariables = tmp.Or(tmp.And(tmp.Not().StaticAdd(one)).Divide(vs.And(vs.Not().StaticAdd(one))).RightShift(1).Subtract(one));
            return nextVariables;
        }

        public ulong ToULong()
        {
            ulong value = 0;
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i])
                {
                    value += (ulong)Math.Pow(2, i);
                }
            }
            return value;
        }

        public int Cardinality()
        {
            int count = 0;
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i])
                {
                    count += 1;
                }
            }
            return count;
        }

        public int PreviousSetBit(int index)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                if (this.Get(i))
                {
                    return i;
                }
            }
            return -1;
        }

        public int LastSetBit()
        {
            return PreviousSetBit(item.Count);
        }

        private void AlignLength(Varset varset)
        {
            if (item.Count > varset.item.Count)
            {
                BitArray tmp = new BitArray(item.Count);
                varset.item.Length = item.Length;
                varset.item = varset.item.Or(tmp);
            }
            else if (item.Count < varset.item.Count)
            {
                BitArray tmp = new BitArray(varset.item.Count);
                item.Length = varset.item.Length;
                varset.item = varset.item.Or(tmp);
            }
        }

        private BitArray item = new BitArray(0);
    }
}
