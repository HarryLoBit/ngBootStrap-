using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiHelp;
using WxPayAPI;

/// <summary>
/// WxPayApi 的摘要说明
/// </summary>
public class WxPayApi
{
    public WxPayApi()
    {
    }

    /**
      *    
      * 查询订单
      * @param WxPayData inputObj 提交给查询订单API的参数
      * @param int timeOut 超时时间
      * @throws WxPayException
      * @return 成功时返回订单查询结果，其他抛异常
      */
    public static WxPayData OrderQuery(WxPayData inputObj, int timeOut = 6)
    {
        string url = "https://api.mch.weixin.qq.com/pay/orderquery";
        //检测必填参数
        if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
        {
            throw new Exception("订单查询接口中，out_trade_no、transaction_id至少填一个！");
        }

        inputObj.SetValue("appid", WxPayConfig.APPID);//公众账号ID
        inputObj.SetValue("mch_id", WxPayConfig.MCHID);//商户号
        inputObj.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
        inputObj.SetValue("sign", inputObj.MakeSign());//签名

        string xml = inputObj.ToXml();

        var start = DateTime.Now;

        //Log.Debug("WxPayApi", "OrderQuery request : " + xml);
        string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据
                                                                     //   Log.Debug("WxPayApi", "OrderQuery response : " + response);

        var end = DateTime.Now;
        int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

        //将xml格式的数据转化为对象以返回
        WxPayData result = new WxPayData();
        result.FromXml(response);

        ReportCostTime(url, timeCost, result);//测速上报

        return result;
    }


    /**
    * 
    * 测速上报
    * @param string interface_url 接口URL
    * @param int timeCost 接口耗时
    * @param WxPayData inputObj参数数组
    */
    private static void ReportCostTime(string interface_url, int timeCost, WxPayData inputObj)
    {
        //如果仅失败上报
        if (WxPayConfig.REPORT_LEVENL == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
         inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
        {
            return;
        }

        //上报逻辑
        WxPayData data = new WxPayData();
        data.SetValue("interface_url", interface_url);
        data.SetValue("execute_time_", timeCost);
        //返回状态码
        if (inputObj.IsSet("return_code"))
        {
            data.SetValue("return_code", inputObj.GetValue("return_code"));
        }
        //返回信息
        if (inputObj.IsSet("return_msg"))
        {
            data.SetValue("return_msg", inputObj.GetValue("return_msg"));
        }
        //业务结果
        if (inputObj.IsSet("result_code"))
        {
            data.SetValue("result_code", inputObj.GetValue("result_code"));
        }
        //错误代码
        if (inputObj.IsSet("err_code"))
        {
            data.SetValue("err_code", inputObj.GetValue("err_code"));
        }
        //错误代码描述
        if (inputObj.IsSet("err_code_des"))
        {
            data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
        }
        //商户订单号
        if (inputObj.IsSet("out_trade_no"))
        {
            data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
        }
        //设备号
        if (inputObj.IsSet("device_info"))
        {
            data.SetValue("device_info", inputObj.GetValue("device_info"));
        }

        try
        {
            Report(data);
        }
        catch// (Exception ex)
        {
            //不做任何处理
        }
    }


    /**
      * 
      * 测速上报接口实现
      * @param WxPayData inputObj 提交给测速上报接口的参数
      * @param int timeOut 测速上报接口超时时间
      * @throws WxPayException
      * @return 成功时返回测速上报接口返回的结果，其他抛异常
      */
    public static WxPayData Report(WxPayData inputObj, int timeOut = 1)
    {
        string url = "https://api.mch.weixin.qq.com/payitil/report";
        //检测必填参数
        if (!inputObj.IsSet("interface_url"))
        {
            throw new Exception("接口URL，缺少必填参数interface_url！");
        }
        if (!inputObj.IsSet("return_code"))
        {
            throw new Exception("返回状态码，缺少必填参数return_code！");
        }
        if (!inputObj.IsSet("result_code"))
        {
            throw new Exception("业务结果，缺少必填参数result_code！");
        }
        if (!inputObj.IsSet("user_ip"))
        {
            throw new Exception("访问接口IP，缺少必填参数user_ip！");
        }
        if (!inputObj.IsSet("execute_time_"))
        {
            throw new Exception("接口耗时，缺少必填参数execute_time_！");
        }

        inputObj.SetValue("appid", WxPayConfig.APPID);//公众账号ID
        inputObj.SetValue("mch_id", WxPayConfig.MCHID);//商户号 
        inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
        inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
        inputObj.SetValue("sign", inputObj.MakeSign());//签名
        string xml = inputObj.ToXml();
        // Log.Info("WxPayApi", "Report request : " + xml); 
        string response = HttpService.Post(xml, url, false, timeOut);

        //        Log.Info("WxPayApi", "Report response : " + response);

        WxPayData result = new WxPayData();
        result.FromXml(response);
        return result;
    }


    /**
       * 生成随机串，随机串包含字母或数字
       * @return 随机串
       */
    public static string GenerateNonceStr()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }



    /**
    * 
    * 统一下单
    * @param WxPaydata inputObj 提交给统一下单API的参数
    * @param int timeOut 超时时间
    * @throws WxPayException
    * @return 成功时返回，其他抛异常
    */
    public static WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
    {
        string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

        //检测必填参数
        if (!inputObj.IsSet("out_trade_no"))
        {
            throw new Exception("缺少统一支付接口必填参数out_trade_no！");
        }
        else if (!inputObj.IsSet("body"))
        {
            throw new Exception("缺少统一支付接口必填参数body！");
        }
        else if (!inputObj.IsSet("total_fee"))
        {
            throw new Exception("缺少统一支付接口必填参数total_fee！");
        }
        else if (!inputObj.IsSet("trade_type"))
        {
            throw new Exception("缺少统一支付接口必填参数trade_type！");
        }

        //关联参数
        if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
        {
            throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");

        }
        if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
        {
            throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
        }

        //异步通知url未设置，则使用配置文件中的url
        if (!inputObj.IsSet("notify_url"))
        {
            inputObj.SetValue("notify_url", ApiHelper.GetWebLocation() + "/api/pay/alinotify");//异步通知url
        }

        inputObj.SetValue("appid", WxPayConfig.APPID);//公众账号ID
        inputObj.SetValue("mch_id", WxPayConfig.MCHID);//商户号
        inputObj.SetValue("spbill_create_ip", ApiHelper.GetClientIP());//终端ip	  	    
        inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

        //签名
        inputObj.SetValue("sign", inputObj.MakeSign());
        string xml = "";
        xml = inputObj.ToXml();
        var start = DateTime.Now;
        string response = "";
        try
        {
            response = HttpService.Post(xml, url, false, timeOut);
        }
        catch (Exception eeee)
        {
            
        }
        var end = DateTime.Now;
        int timeCost = (int)((end - start).TotalMilliseconds);

        WxPayData result = new WxPayData();
        result.FromXml(response);
        ReportCostTime(url, timeCost, result);//测速上报

        return result;
    }
}