using System;

#if UNITY_2020_1_OR_NEWER
public static class MathExt
{
    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref byte self, int value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked((byte)(self + value)),
                1 => checked((byte)(self - value)),
                2 => checked((byte)(self * value)),
                3 => checked((byte)(self / value)),
                4 => checked((byte)(self % value)),
                5 => checked((byte)(self ^ value)),
                6 => checked((byte)(self & value)),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref sbyte self, int value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked((sbyte)(self + value)),
                1 => checked((sbyte)(self - value)),
                2 => checked((sbyte)(self * value)),
                3 => checked((sbyte)(self / value)),
                4 => checked((sbyte)(self % value)),
                5 => checked((sbyte)(self ^ value)),
                6 => checked((sbyte)(self & value)),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref short self, int value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked((short)(self + value)),
                1 => checked((short)(self - value)),
                2 => checked((short)(self * value)),
                3 => checked((short)(self / value)),
                4 => checked((short)(self % value)),
                5 => checked((short)(self ^ value)),
                6 => checked((short)(self & value)),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref ushort self, int value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked((ushort)(self + value)),
                1 => checked((ushort)(self - value)),
                2 => checked((ushort)(self * value)),
                3 => checked((ushort)(self / value)),
                4 => checked((ushort)(self % value)),
                5 => checked((ushort)(self ^ value)),
                6 => checked((ushort)(self & value)),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static int CalcRef(this ref int self, int value, byte oper)
    {
        return self = Calc(self, value, oper);
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static int Calc(this int self, int value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                5 => checked(self ^ value),
                6 => checked(self & value),
                _ => throw new Exception("没有这个操作数"),
            };
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return self;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref uint self, uint value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                5 => checked(self ^ value),
                6 => checked(self & value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref float self, float value, byte oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref long self, int value, int oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                5 => checked(self ^ value),
                6 => checked(self & value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref ulong self, ulong value, int oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                5 => checked(self ^ value),
                6 => checked(self & value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref double self, double value, int oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }

    /// <summary>
    /// 检查算术溢出，如果溢出了，啥都不能做
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <param name="oper">0：加 1：减 2：乘 3：除 4：取模 5：异或 6：与</param>
    /// <returns></returns>
    public static bool Calc(this ref decimal self, decimal value, int oper)
    {
        try
        {
            self = oper switch
            {
                0 => checked(self + value),
                1 => checked(self - value),
                2 => checked(self * value),
                3 => checked(self / value),
                4 => checked(self % value),
                _ => throw new Exception("没有这个操作数"),
            };
            return true;
        }
        catch (Exception ex)
        {
            Net.Event.NDebug.LogError("算术溢出:" + ex);
        }
        return false;
    }
}
#endif