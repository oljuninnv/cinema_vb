Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.IO
Imports System.Net

Public Class Form1
    Private apiURL As String = "http://127.0.0.1:8000/test"
    Private filmList As New List(Of String)
    Private hallList As New List(Of String)
    Private timeBeginList As New List(Of String)
    Private priceList As New List(Of Decimal)
    Private dateSeansList As New List(Of Date)
    Private seansList As New List(Of Object)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        getDataFromAPI()
    End Sub

    Private Sub getDataFromAPI()
        Try
            Dim request As HttpWebRequest = WebRequest.Create(apiURL)
            request.Method = "GET"
            request.ContentType = "application/json"

            Dim response As HttpWebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)

            Dim responseFromServer As String = reader.ReadToEnd()
            Dim seansList As List(Of Object) = JsonConvert.DeserializeObject(Of List(Of Object))(responseFromServer)


            For Each item In seansList
                Dim film As String = item("film")
                Dim hall As String = item("hall")
                Dim dateSeans As Date = item("date")
                Dim timeBegin As DateTime = item("time_begin")
                Dim priceSeans As String = item("price_seans")
                Dim status As Boolean = item("status")

                If Not filmList.Contains(film) Then
                    filmList.Add(film)
                    ComboBox1.Items.Add(film)
                End If
                If Not hallList.Contains(hall) Then
                    hallList.Add(hall)
                    ComboBox2.Items.Add(hall)
                End If
                If Not timeBeginList.Contains(timeBegin) Then
                    timeBeginList.Add(timeBegin)
                    ComboBox3.Items.Add(timeBegin)
                End If
                If Not priceList.Contains(priceSeans) Then
                    priceList.Add(priceSeans)
                    ComboBox4.Items.Add(priceSeans)
                End If
                If Not dateSeansList.Contains(dateSeans) Then
                    dateSeansList.Add(dateSeans)
                End If
            Next

            reader.Close()
            dataStream.Close()
            response.Close()

        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim response As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                If response.StatusCode = HttpStatusCode.NotFound Then
                    MessageBox.Show("Seans matching query does not exist.")
                End If
            End If
        End Try
    End Sub

    Private Sub cbFilm_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        filterData()
    End Sub

    Private Sub dtpDate_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        filterData()
    End Sub

    Private Sub cbTimeBegin_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        filterData()
    End Sub

    Private Sub cbPrice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        filterData()
    End Sub

    Private Sub filterData()
        Dim selectedFilm As String = ComboBox1.Text
        Dim selectedDate As Date = DateTimePicker1.Value.Date
        Dim selectedTimeBegin As String = ComboBox3.Text
        Dim selectedPrice As Decimal = ComboBox4.Text
        Dim statusSeans As Boolean = False


        If String.IsNullOrEmpty(selectedFilm) Or selectedDate = Nothing Or String.IsNullOrEmpty(selectedTimeBegin) Or selectedPrice = 0 Then
            Return
        End If

        For Each item In seansList
            Dim film As String = item("film")
            Dim hall As String = item("hall")
            Dim timeBegin As String = item("time_begin")
            Dim priceSeans As Decimal = item("price_seans")
            Dim dateSeans As Date = item("date_of_seans")
            statusSeans = item("status")

            If film = selectedFilm And dateSeans = selectedDate And timeBegin = selectedTimeBegin And priceSeans = selectedPrice Then
                If statusSeans = True Then
                    TextBox1.Text = "All seats occupied"
                Else
                    TextBox1.Text = "Not all seats are occupied"
                End If
                Return
            End If
        Next
    End Sub
End Class
