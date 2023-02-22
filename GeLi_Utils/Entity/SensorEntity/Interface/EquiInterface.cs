using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiService_WMS.Entity.SensorEntity
{
    /// <summary>
    /// 设备接口
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 设备站号
        /// </summary>
        int StationAddress { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        string EquipmentName { get; }

        
        /// <summary>
        /// 接收到的消息
        /// </summary>
        string ReceiveMessage { get; set; }

        /// <summary>
        /// 读取数据的报文
        /// </summary>
        byte[] CommandCode { get; }
    }

    /// <summary>
    /// 传感器
    /// </summary>
    /// <typeparam name="T">读取到的数据的类型</typeparam>
    public interface ISensor<T>
    {
        /// <summary>
        /// 计算数据结果
        /// </summary>
        /// <param name="msg">接收到的消息</param>
        /// <returns></returns>
        T GetResult();
    }

    /// <summary>
    /// 控制器
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 开启控制器（吸合继电器）的报文
        /// </summary>
        byte[] OpenCmdCode { get; }

        /// <summary>
        /// 关闭控制器（断开继电器）的报文
        /// </summary>
        byte[] CloseCmdeCode { get; }

        /// <summary>
        /// 控制器状态（开/关）
        /// </summary>
        bool Status { get; set; }
    }
}
