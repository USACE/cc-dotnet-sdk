using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  public static class EnvironmentVariables
  {
    public static String CC_MANIFEST_ID = "CC_MANIFEST_ID";
    public static String CC_EVENT_NUMBER = "CC_EVENT_NUMBER";
    public static String CC_EVENT_ID = "CC_EVENT_ID";
    public static String CC_ROOT = "CC_ROOT"; 
    public static String CC_PLUGIN_DEFINITION = "CC_PLUGIN_DEFINITION";
    public static String CC_PROFILE = "CC";
    public static String CC_PAYLOAD_FORMATTED = "CC_PAYLOAD_FORMATTED";
    public static String AWS_ACCESS_KEY_ID = "AWS_ACCESS_KEY_ID";
    public static String AWS_SECRET_ACCESS_KEY = "AWS_SECRET_ACCESS_KEY";
    public static String AWS_DEFAULT_REGION = "AWS_DEFAULT_REGION";
    public static String AWS_S3_BUCKET = "AWS_S3_BUCKET";
  }
}
