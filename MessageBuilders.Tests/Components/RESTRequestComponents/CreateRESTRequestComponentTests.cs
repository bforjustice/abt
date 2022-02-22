namespace MessageBuilders.Components.RESTRequestComponents.Tests
{
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Components.RESTRequestComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Net;

    public class CreateRESTRequestComponentTests
    {
        [TestMethod()]
        public void DoTest()
        {
            HttpWebRequest req = null;

            ParameterComponent apiKeyBlock = new ParameterComponent("apiKey", "String");
            ParameterComponent keyBlock = new ParameterComponent("SecretKey", "String");

            ParameterComponent coinType = new ParameterComponent("symbol", "String");
            ParameterComponent sideBlock = new ParameterComponent("side", "String");
            ParameterComponent orderTypeBlock = new ParameterComponent("type", "String");
            ParameterComponent timeInForceBlock = new ParameterComponent("timeInForce", "String");
            ParameterComponent qtyBlock = new ParameterComponent("quantity", "String");
            ParameterComponent priceBlock = new ParameterComponent("price", "String");
            ParameterComponent recvWindowBlock = new ParameterComponent("recvWindow", "String");
            ParameterComponent nounceBlock = new ParameterComponent("timestamp", "String");
            ParameterComponent signatureBlock = new ParameterComponent("signature", "String");

            IBlockComponent<object> baseBlock = new ValueComponent("BaseUrl", "https://fapi.binance.com");
            IBlockComponent<object> prefixBlock = new ValueComponent("Prefix", "/fapi/v1/order");
            IBlockComponent<object> delimeterBlock = new ValueComponent("Delimeter", "?");
            IBlockComponent<object> methodBlock = new PostMethodComponent("Method");

            coinType.SetValue("BTCUSDT");
            sideBlock.SetValue("BUY");
            orderTypeBlock.SetValue("LIMIT");
            timeInForceBlock.SetValue("GTC");
            priceBlock.SetValue("9000");
            qtyBlock.SetValue("1");
            recvWindowBlock.SetValue("5000");
            nounceBlock.SetValue("1591702613943");

            apiKeyBlock.SetValue("dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83");
            keyBlock.SetValue("2b5eb11e18796d12d88f13dc27dbbd02c2cc51ff7059765ed9821957d82bb4d9");

            IBlockComponent<object> querycombiner = new CombineQueryString("QueryCombiner");
            querycombiner.SetSubComponent(coinType);
            querycombiner.SetSubComponent(sideBlock);
            querycombiner.SetSubComponent(orderTypeBlock);
            querycombiner.SetSubComponent(qtyBlock);
            querycombiner.SetSubComponent(priceBlock);
            querycombiner.SetSubComponent(timeInForceBlock);
            querycombiner.SetSubComponent(recvWindowBlock);
            querycombiner.SetSubComponent(nounceBlock);

            IBlockComponent<object> requestPathCombiner = new CombineRequestPathComponent("RequestPathCombiner");
            requestPathCombiner.SetSubComponent(baseBlock);
            requestPathCombiner.SetSubComponent(prefixBlock);
            Assert.IsTrue(Convert.ToString(requestPathCombiner.Result).Equals("https://fapi.binance.com/fapi/v1/order"));

            SHA256Component hash = new SHA256Component("Hash");
            hash.SetSubComponent(keyBlock);
            hash.SetSubComponent(querycombiner);

            ByteToStringEncodingComponent encoded = new ByteToStringEncodingComponent("signature");
            encoded.SetSubComponent(hash);
            Assert.IsTrue(Convert.ToString(encoded.Result).Equals("3c661234138461fcc7a7d8746c6558c9842d4e10870d2ecbedf7777cad694af9"));

            IBlockComponent<object> signatureCombine = new CombineQueryString("SignatureCombine");
            signatureCombine.SetSubComponent(querycombiner);
            signatureCombine.SetSubComponent(encoded);

            IBlockComponent<object> fullPathCombiner = new CombineFullPathComponent("FullPathCombiner");
            fullPathCombiner.SetSubComponent(requestPathCombiner);
            fullPathCombiner.SetSubComponent(delimeterBlock);
            fullPathCombiner.SetSubComponent(signatureCombine);
            Assert.IsTrue(Convert.ToString(fullPathCombiner.Result).Equals("https://fapi.binance.com/fapi/v1/order?symbol=BTCUSDT&side=BUY&type=LIMIT&quantity=1&price=9000&timeInForce=GTC&recvWindow=5000&timestamp=1591702613943&signature=3c661234138461fcc7a7d8746c6558c9842d4e10870d2ecbedf7777cad694af9"));

            CreateRESTRequestComponent comp = new CreateRESTRequestComponent("CreateHttpRequest");
            comp.SetSubComponent(fullPathCombiner);

            InsertContentTypeComponent comp_1 = new InsertContentTypeComponent("InsertContentTypeComponent", "application /x-www-form-urlencoded");
            InsertHeaderComponent comp_2 = new InsertHeaderComponent("X-MBX-APIKEY");
            InsertAcceptComponent comp_3 = new InsertAcceptComponent("InsertAcceptComponent", "application /json");
            InsertMethodComponent comp_4 = new InsertMethodComponent("Method");
            CreatePostMessageComponent comp_5 = new CreatePostMessageComponent("StreamWriter");

            comp.SetSubComponent(fullPathCombiner);
            comp_2.SetSubComponent(apiKeyBlock);
            comp_4.SetSubComponent(methodBlock);
            comp_5.SetSubComponent(signatureCombine);

            req = comp.Do(req);
            req = comp_2.Do(req);
            req = comp_3.Do(req);
            req = comp_4.Do(req);
            req = comp_5.Do(req);

            Assert.IsTrue(req.RequestUri.Equals("https://fapi.binance.com/fapi/v1/order?symbol=BTCUSDT&side=BUY&type=LIMIT&quantity=1&price=9000&timeInForce=GTC&recvWindow=5000&timestamp=1591702613943&signature=3c661234138461fcc7a7d8746c6558c9842d4e10870d2ecbedf7777cad694af9"));
            Assert.IsTrue(req.Headers["X-MBX-APIKEY"].Equals("dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83"));
            Assert.IsTrue(req.Method.Equals("POST"));
        }
    }
}