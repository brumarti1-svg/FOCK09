Imports System.Runtime.InteropServices
Imports System.Text

Module Wininet

    Public Const scUserAgent As String = "http sample"

    Public Const INTERNET_OPEN_TYPE_PRECONFIG As Integer = 0

    Public Const INTERNET_DEFAULT_FTP_PORT As Integer = 21
    Public Const INTERNET_DEFAULT_GOPHER_PORT As Integer = 70
    Public Const INTERNET_DEFAULT_HTTP_PORT As Integer = 80
    Public Const INTERNET_DEFAULT_HTTPS_PORT As Integer = 443
    Public Const INTERNET_DEFAULT_SOCKS_PORT As Integer = 1080

    Public Const INTERNET_SERVICE_FTP As Integer = 1
    Public Const INTERNET_SERVICE_GOPHER As Integer = 2
    Public Const INTERNET_SERVICE_HTTP As Integer = 3

    Public Const INTERNET_FLAG_RELOAD As Integer = &H80000000

    Public Const INTERNET_OPTION_VERSION As Integer = 40

    Public Const HTTP_QUERY_CONTENT_TYPE As Integer = 1
    Public Const HTTP_QUERY_CONTENT_LENGTH As Integer = 5
    Public Const HTTP_QUERY_EXPIRES As Integer = 10
    Public Const HTTP_QUERY_LAST_MODIFIED As Integer = 11
    Public Const HTTP_QUERY_PRAGMA As Integer = 17
    Public Const HTTP_QUERY_VERSION As Integer = 18
    Public Const HTTP_QUERY_STATUS_CODE As Integer = 19
    Public Const HTTP_QUERY_STATUS_TEXT As Integer = 20
    Public Const HTTP_QUERY_RAW_HEADERS As Integer = 21
    Public Const HTTP_QUERY_RAW_HEADERS_CRLF As Integer = 22
    Public Const HTTP_QUERY_FORWARDED As Integer = 30
    Public Const HTTP_QUERY_SERVER As Integer = 37
    Public Const HTTP_QUERY_USER_AGENT As Integer = 39
    Public Const HTTP_QUERY_SET_COOKIE As Integer = 43
    Public Const HTTP_QUERY_REQUEST_METHOD As Integer = 45

    Public Const HTTP_QUERY_FLAG_REQUEST_HEADERS As Integer = &H80000000

    Public Const HTTP_ADDREQ_FLAG_ADD_IF_NEW As Integer = &H10000000
    Public Const HTTP_ADDREQ_FLAG_ADD As Integer = &H20000000
    Public Const HTTP_ADDREQ_FLAG_REPLACE As Integer = &H80000000

    <StructLayout(LayoutKind.Sequential)>
    Public Structure WinInetDLLVersion
        Public lMajorVersion As Integer
        Public lMinorVersion As Integer
    End Structure

    <DllImport("wininet.dll", EntryPoint:="InternetOpenA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function InternetOpen(
        ByVal sAgent As String,
        ByVal lAccessType As Integer,
        ByVal sProxyName As String,
        ByVal sProxyBypass As String,
        ByVal lFlags As Integer) As IntPtr
    End Function

    <DllImport("wininet.dll", EntryPoint:="InternetConnectA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function InternetConnect(
        ByVal hSession As IntPtr,
        ByVal sServerName As String,
        ByVal nServerPort As Integer,
        ByVal sUsername As String,
        ByVal sPassword As String,
        ByVal lService As Integer,
        ByVal lFlags As Integer,
        ByVal lContext As IntPtr) As IntPtr
    End Function

    <DllImport("wininet.dll", EntryPoint:="HttpOpenRequestA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function HttpOpenRequest(
        ByVal hConnect As IntPtr,
        ByVal sVerb As String,
        ByVal sObjectName As String,
        ByVal sVersion As String,
        ByVal sReferer As String,
        ByVal ppszAcceptTypes As IntPtr,
        ByVal lFlags As Integer,
        ByVal lContext As IntPtr) As IntPtr
    End Function

    <DllImport("wininet.dll", EntryPoint:="HttpSendRequestA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function HttpSendRequest(
        ByVal hRequest As IntPtr,
        ByVal sHeaders As String,
        ByVal lHeadersLength As Integer,
        ByVal pOptional As IntPtr,
        ByVal lOptionalLength As Integer) As Integer
    End Function

    <DllImport("wininet.dll", EntryPoint:="HttpQueryInfoA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function HttpQueryInfo(
        ByVal hRequest As IntPtr,
        ByVal lInfoLevel As Integer,
        ByVal sBuffer As StringBuilder,
        ByRef lBufferLength As Integer,
        ByRef lIndex As Integer) As Integer
    End Function

    <DllImport("wininet.dll", SetLastError:=True)>
    Public Function InternetReadFile(
        ByVal hFile As IntPtr,
        ByVal pBuffer As Byte(),
        ByVal lNumBytesToRead As Integer,
        ByRef lNumberOfBytesRead As Integer) As Integer
    End Function

    <DllImport("wininet.dll", SetLastError:=True)>
    Public Function InternetCloseHandle(ByVal hInet As IntPtr) As Integer
    End Function

    <DllImport("wininet.dll", EntryPoint:="InternetQueryOptionA", SetLastError:=True)>
    Public Function InternetQueryOption(
        ByVal hInternet As IntPtr,
        ByVal lOption As Integer,
        ByRef sBuffer As WinInetDLLVersion,
        ByRef lBufferLength As Integer) As Integer
    End Function

    <DllImport("wininet.dll", EntryPoint:="HttpAddRequestHeadersA", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Function HttpAddRequestHeaders(
        ByVal hRequest As IntPtr,
        ByVal sHeaders As String,
        ByVal lHeadersLength As Integer,
        ByVal lModifiers As Integer) As Integer
    End Function

End Module
