//======================================================================
//      Copyright (c) 2021-04-16 Zeiot All rights reserved. 
//======================================================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeiot.Model.DBContext
 { 
    /// <summary> 
    /// 
    /// <summary> 
    [Table("a_shebei")]
    public class a_shebei
    { 

        /// <summary>
        ///  自增主键
        /// </summary>
        [Key]
        [Base.Property("Identity")]
        [Required]
        public int id { get; set; }

        /// <summary>
        /// 信号强度
        /// </summary>
        public int xhqd { get; set; }

        /// <summary>
        /// 定时上传间隔
        /// </summary>
        public int scjg { get; set; }

        /// <summary>
        /// 注册序列号
        /// </summary>
        public string xlh { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string kh { get; set; }

        /// <summary>
        /// 固件版本
        /// </summary>
        public string gjbb1 { get; set; }

        /// <summary>
        /// 固件版本
        /// </summary>
        public string gjbb2 { get; set; }

        /// <summary>
        /// 固件版本
        /// </summary>
        public string gjbb3 { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string cjsj { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string xgsj { get; set; }

        /// <summary>
        /// 设备ip地址
        /// </summary>
        public string ipdz { get; set; }
    } 
 } 
