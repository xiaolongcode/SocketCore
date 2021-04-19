//======================================================================
//      Copyright (c) 2021-04-16 Zeiot All rights reserved. 
//======================================================================
using System;
using System.Collections.Generic;
using Zeiot.Model.Base;
using Zeiot.Model.DBContext;

namespace Zeiot.Model.ViewContext
 { 
    /// <summary> 
    /// 
    /// <summary> 
    public class AShebei_View: BizPage
    { 

        /// <summary>
        ///表对象 
        /// </summary>
        public a_shebei  AShebei_info { get; set; }

        /// <summary>
        ///表list 
        /// </summary>
        public List<a_shebei>  AShebei_list { get; set; }

    } 
 } 
