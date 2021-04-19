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
    [Table("a_dianliang")]
    public class a_dianliang
    { 

        /// <summary>
        ///  自增主键
        /// </summary>
        [Key]
        [Base.Property("Identity")]
        [Required]
        public int id { get; set; }

        /// <summary>
        /// 设备id
        /// </summary>
        public int shebenid { get; set; }

        /// <summary>
        /// 电流
        /// </summary>
        public decimal dl1 { get; set; }

        /// <summary>
        /// 电流
        /// </summary>
        public decimal dl2 { get; set; }

        /// <summary>
        /// 电流
        /// </summary>
        public decimal dl3 { get; set; }

        /// <summary>
        /// 相电压
        /// </summary>
        public decimal xdy1 { get; set; }

        /// <summary>
        /// 相电压
        /// </summary>
        public decimal xdy2 { get; set; }

        /// <summary>
        /// 相电压
        /// </summary>
        public decimal xdy3 { get; set; }

        /// <summary>
        /// 总用电量
        /// </summary>
        public decimal zydl { get; set; }

        /// <summary>
        /// 线电压
        /// </summary>
        public decimal xiandy1 { get; set; }

        /// <summary>
        /// 线电压
        /// </summary>
        public decimal xiandy2 { get; set; }

        /// <summary>
        /// 线电压
        /// </summary>
        public decimal xiandy3 { get; set; }

        /// <summary>
        /// 用电量
        /// </summary>
        public decimal aydl { get; set; }

        /// <summary>
        /// 用电量
        /// </summary>
        public decimal bydl { get; set; }

        /// <summary>
        /// 用电量
        /// </summary>
        public decimal cydl { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string date { get; set; }
    } 
 } 
