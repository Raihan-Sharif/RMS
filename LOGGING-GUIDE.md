# 🎯 Enterprise-Grade Logging Configuration Guide

## 📋 **Overview**
SimRMS now features a **professional, configurable logging system** designed to minimize log size while maintaining essential debugging information. You have **complete control** over what gets logged through simple configuration settings.

---

## 🔧 **Quick Configuration**

### **Production Settings (Minimal Logging)**
```json
{
  "LoggingSettings": {
    "RequestResponse": {
      "LogSuccessfulRequests": false,
      "LogRequestBodies": false,
      "LogResponseBodies": false,
      "LogHeaders": false,
      "SlowRequestThresholdMs": 2000
    },
    "Security": {
      "LogValidationErrors": false
    },
    "Database": {
      "LogSuccessfulOperations": false,
      "LogParameters": false
    }
  }
}
```

### **Development Settings (Detailed Logging)**
```json
{
  "LoggingSettings": {
    "RequestResponse": {
      "LogSuccessfulRequests": true,
      "LogRequestBodies": true,
      "LogResponseBodies": true,
      "SlowRequestThresholdMs": 500
    },
    "Security": {
      "LogValidationErrors": true
    },
    "Database": {
      "LogSuccessfulOperations": false,
      "LogParameters": false
    }
  }
}
```

---

## 🎛️ **Complete Configuration Options**

### **RequestResponse Logging**
| Setting | Production | Development | Description |
|---------|------------|-------------|-------------|
| `LogSuccessfulRequests` | `false` | `true` | Log 2xx status responses |
| `LogRequestBodies` | `false` | `true` | Log request JSON bodies |
| `LogResponseBodies` | `false` | `true` | Log response JSON bodies |
| `LogHeaders` | `false` | `false` | Log HTTP headers |
| `MaxBodySizeToLog` | `512` | `2048` | Max body size (bytes) |
| `SlowRequestThresholdMs` | `2000` | `500` | Slow request threshold |

### **Security Logging**
| Setting | Always Logs | Description |
|---------|-------------|-------------|
| `LogAuthenticationEvents` | ✅ Always | Cannot be disabled - security critical |
| `LogAuthorizationFailures` | ✅ Always | Authentication failures |
| `LogValidationErrors` | ⚙️ Configurable | Can be noisy in production |
| `LogSuspiciousActivity` | ✅ Always | Security threats |

### **Database Logging**
| Setting | Production | Development | Description |
|---------|------------|-------------|-------------|
| `LogSuccessfulOperations` | `false` | `false` | Log successful DB calls |
| `LogDatabaseErrors` | ✅ Always | Cannot be disabled |
| `LogParameters` | `false` | `false` | **Security risk** - keep false |
| `SlowDatabaseThresholdMs` | `1000` | `200` | Slow query threshold |

### **Framework Noise Suppression**
| Setting | Production | Development | Description |
|---------|------------|-------------|-------------|
| `SuppressFrameworkNoise` | `true` | `true` | Suppress ASP.NET Core noise |
| `LogHttpClientRequests` | `false` | `false` | Can be very verbose |
| `LogAspNetCorePipeline` | `false` | `false` | Internal request processing |

---

## 📊 **What Gets Logged By Default**

### **✅ Always Logged (Cannot Disable)**
- ❌ **All HTTP 4xx/5xx errors** 
- 🗄️ **Database errors and timeouts**
- 🚨 **System exceptions and critical errors**
- ⚠️ **Authentication/authorization failures**
- 🐌 **Slow requests above threshold**

### **⚙️ Configurable Logging**
- ✅ **Successful HTTP requests** (2xx responses)
- 📝 **Request/response bodies** 
- 🔧 **Validation errors** 
- 🗄️ **Successful database operations**
- 📊 **Performance metrics**

### **🚫 Suppressed by Default**
- ASP.NET Core internal logging
- Static file serving logs  
- Health check requests
- Swagger/framework requests
- HTTP client verbose logs

---

## 📁 **Log File Structure**

### **Production Logs** (`C:\Logs\SimRMS\`)
```
├── Application\
│   └── rms-app-20250902.log          # General application logs
├── Errors\
│   └── rms-errors-20250902.log       # Warnings & errors only  
└── Performance\
    └── rms-performance-20250902.log  # Performance tracking
```

### **Development Logs** (`logs/dev/`)
```
└── dev\
    └── rms-dev-20250902.log          # Combined development logs
```

---

## 🎯 **Log Examples**

### **Minimal Production Logging**
```
[2025-09-02 14:30:15.123] [INF] ✅ POST /api/v1/auth/login → 200 (145.2ms)
[2025-09-02 14:30:45.456] [WRN] 🐌 SLOW REQUEST: GET /api/v1/companies took 2341.7ms (threshold: 2000ms)
[2025-09-02 14:31:12.789] [ERR] ❌ POST /api/v1/traders → 400 (23.1ms)
[2025-09-02 14:31:23.012] [ERR] 🗄️ DATABASE ERROR: SQL 2 in POST /api/v1/branches | User: EFTEST03 | Message: Timeout expired
```

### **Development Logging (More Verbose)**
```
[2025-09-02 14:30:15.123] [INF] ✅ POST /api/v1/auth/login → 200 (145.2ms)
[2025-09-02 14:30:15.125] [DBG] Request Body: {"username":"EFTEST03","password":"password"}
[2025-09-02 14:30:15.268] [DBG] Response Body: {"success":true,"data":{"userToken":"..."},"timestamp":"..."}
[2025-09-02 14:30:45.456] [WRN] 🐌 SLOW REQUEST: GET /api/v1/companies took 2341.7ms (threshold: 500ms)
[2025-09-02 14:31:12.789] [WRN] 🔧 VALIDATION ERROR: Company code is required in POST /api/v1/traders | User: EFTEST03
```

---

## 🚀 **Environment Presets**

Use these predefined configurations for different environments:

### **Production Preset** 
- ❌ No successful request logging
- ❌ No request/response bodies  
- ❌ No validation error logging
- ✅ Only errors, security events, and slow requests

### **Development Preset**
- ✅ Log successful requests
- ✅ Log request/response bodies (up to 2KB)
- ✅ Log validation errors
- 🐌 Lower slow request threshold (500ms)

### **Debug Preset** 
- ✅ Maximum logging for troubleshooting
- ✅ Log all requests, bodies, headers
- ✅ Log database parameters (**security risk**)
- 🐌 Very low thresholds (100ms)

---

## 📝 **Configuration Tips**

### **🔒 Security Best Practices**
- ❌ **Never log database parameters in production** (contains sensitive data)
- ❌ **Never log request/response bodies in production** (may contain passwords)  
- ❌ **Never log HTTP headers** (may contain tokens)
- ✅ **Always keep authentication/authorization logging enabled**

### **📉 Performance Optimization**
- Set higher thresholds in production (2000ms vs 500ms)
- Disable successful request logging in production
- Use async logging for better performance
- Exclude static files and health checks

### **🐞 Debugging**
- Enable request/response body logging temporarily
- Lower slow request thresholds 
- Enable validation error logging
- Use development preset for troubleshooting

---

## 🛠️ **Runtime Configuration Changes**

You can change logging settings **without restarting** the application:

1. **Modify** `appsettings.json` or `appsettings.Production.json`
2. **Settings reload automatically** within ~30 seconds
3. **New requests** use updated configuration

---

## 📊 **Log Size Estimates**

| Configuration | Requests/Day | Estimated Daily Log Size |
|---------------|--------------|-------------------------|
| **Production** | 10,000 | ~10-50 MB |
| **Development** | 1,000 | ~50-200 MB |
| **Debug** | 1,000 | ~200-500 MB |

---

## 🚨 **Emergency Debugging**

If you need **maximum logging** temporarily for production debugging:

```json
{
  "LoggingSettings": {
    "RequestResponse": {
      "LogSuccessfulRequests": true,
      "LogRequestBodies": true,
      "LogResponseBodies": true,
      "SlowRequestThresholdMs": 100
    },
    "Security": {
      "LogValidationErrors": true
    },
    "Database": {
      "LogSuccessfulOperations": true
    }
  }
}
```

**⚠️ Remember to revert after debugging - this will create large log files!**

---

Your logging system is now **enterprise-ready** with **full control** over what gets logged! 🎯