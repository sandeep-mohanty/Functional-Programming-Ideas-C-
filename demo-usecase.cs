/*---------------------------------SAMPLE USECASE-----------------------------------------------------
A hypothetical usecase to show the usage of higher order function with function composition techniques 
to create business logic pipelines that drives business transactions. The techniques
involves creation of single responsibility functions and then combining them to get the desired 
results.

The design concepts explained here visualizes a multi-service/multi-component environment 
(microservices, SOA or modules within the same application). The demo here shows a very basic pub-sub 
based messaging system in action with the help function composition to create transaction pipelines.
-----------------------------------SAMPLE USECASE-----------------------------------------------------*/
using System;
using System.Threading;
using System.Collections.Generic;
using static System.Console;
using Utilities;

namespace Demo {
  public class DemoUsecase {

    // Dummy messaging system which conveys important events to isolated systems
    private readonly MessageBroker<string> MESSAGE_SERVICE;

    // Message queues (Different service listen to these queues based on their needs)
    private readonly string HR_QUEUE_NAME = "HR_SERVICE: EMAIL_CREATED";
    private readonly string FINANCE_QUEUE_NAME = "FINANCE_SERVICE: EMAIL_CREATED";
    private readonly string IT_QUEUE_NAME = "IT_SERVICE: EMAIL_CREATED";
    private readonly string RESEARCH_QUEUE_NAME = "RESEARCH_SERVICE: EMAIL_CREATED";

    // Specific event listener for multiple services in the services eco-system
    private readonly Action<string> HR_SERVICE_LISTENER;
    private readonly Action<string> FINANCE_SERVICE_LISTENER;
    private readonly Action<string> IT_SERVICE_LISTENER;
    private readonly Action<string> RESEARCH_SERVICE_LISTENER;

    // Event dispatchers that post messages to the message queue when something interesting happens
    private readonly Func<Func<string, string>, Func<string, string>> HR_EVENT_DISPATCHER;
    private readonly Func<Func<string, string>, Func<string, string>> FINANCE_EVENT_DISPATCHER;
    private readonly Func<Func<string, string>, Func<string, string>> IT_EVENT_DISPATCHER;
    private readonly Func<Func<string, string>, Func<string, string>> RESEARCH_EVENT_DISPATCHER;

    // Transaction pipeline variables
    public Func<string,string> startCreateEmailTransaction {get;}
    public Func<string,string> startAnnouncement{get;}

    public DemoUsecase() {

      /*----------------------Messaging system setup-----------------------------------------*/
      MESSAGE_SERVICE = new MessageBroker<string>();

      // Initialize dispatchers
      HR_EVENT_DISPATCHER = (Func<string, string> fn) => {
        return (string args) => {
            string result = fn(args);
            Thread.Sleep(1000);
            MESSAGE_SERVICE.sendMessage(HR_QUEUE_NAME, result);
            return result;
          };
      };

      FINANCE_EVENT_DISPATCHER = (Func<string, string> fn) => {
        return (string args) => {
            string result = fn(args);
            Thread.Sleep(1000);
            MESSAGE_SERVICE.sendMessage(FINANCE_QUEUE_NAME, result);
            return result;
          };
      };

      IT_EVENT_DISPATCHER = (Func<string, string> fn) => {
        return (string args) => {
            string result = fn(args);
            Thread.Sleep(1000);
            MESSAGE_SERVICE.sendMessage(IT_QUEUE_NAME, result);
            return result;
          };
      };

      RESEARCH_EVENT_DISPATCHER = (Func<string, string> fn) => {
        return (string args) => {
            string result = fn(args);
            Thread.Sleep(1000);
            MESSAGE_SERVICE.sendMessage(RESEARCH_QUEUE_NAME, result);
            return result;
          };
      };

      // Intitialize listeners
      HR_SERVICE_LISTENER = (string message) => WriteLine($"HR SERVICE received message: {message}");
      FINANCE_SERVICE_LISTENER = (string message) => WriteLine($"FINANCE SERVICE received message: {message}");
      RESEARCH_SERVICE_LISTENER = (string message) => WriteLine($"R & D SERVICE received message: {message}");
      IT_SERVICE_LISTENER = (string message) => {
          WriteLine($"IT SERVICE received message: {message}");
          if ( message == "malicious.user@lexmark.com") {
            List<Func<Func<string, string>, Func<string, string>>> deleteActions = 
                new List<Func<Func<string, string>, Func<string, string>>>() {IT_EVENT_DISPATCHER, HR_EVENT_DISPATCHER};
            Composer<string, string> deletePipelineComposer = new Composer<string, string>(deleteActions);
            var startDeleteEmailTransaction = deletePipelineComposer.CreatePipeline(deleteEmail);

            List<Func<Func<string, string>, Func<string, string>>> broadcastActions = 
                new List<Func<Func<string, string>, Func<string, string>>>() {HR_EVENT_DISPATCHER, IT_EVENT_DISPATCHER, FINANCE_EVENT_DISPATCHER, RESEARCH_EVENT_DISPATCHER};
            Composer<string, string> broadcastPipelineComposer = new Composer<string, string>(broadcastActions);
            var startBroadcasting = broadcastPipelineComposer.CreatePipeline(announcement);

            startBroadcasting($"SECURITY ALERT: The email address {message} has been blocked due to security reasons");
            Thread.Sleep(10000);
            startDeleteEmailTransaction($"{message}");
          }
        };

      // Register the service listeners to listen to interested queues
      MESSAGE_SERVICE.addSubscription(HR_QUEUE_NAME, HR_SERVICE_LISTENER);
      MESSAGE_SERVICE.addSubscription(FINANCE_QUEUE_NAME, FINANCE_SERVICE_LISTENER);
      MESSAGE_SERVICE.addSubscription(IT_QUEUE_NAME, IT_SERVICE_LISTENER);
      MESSAGE_SERVICE.addSubscription(RESEARCH_QUEUE_NAME, RESEARCH_SERVICE_LISTENER);

      // Initialize business pipelines for later use
      List<Func<Func<string, string>, Func<string, string>>> emailActions = 
          new List<Func<Func<string, string>, Func<string, string>>>() {IT_EVENT_DISPATCHER, HR_EVENT_DISPATCHER, FINANCE_EVENT_DISPATCHER};
      Composer<string, string> emailPipelineComposer = new Composer<string, string>(emailActions);
      startCreateEmailTransaction = emailPipelineComposer.CreatePipeline(createEmail);

      List<Func<Func<string, string>, Func<string, string>>> announcementActions = 
          new List<Func<Func<string, string>, Func<string, string>>>() {HR_EVENT_DISPATCHER, IT_EVENT_DISPATCHER, FINANCE_EVENT_DISPATCHER, RESEARCH_EVENT_DISPATCHER};
      Composer<string, string> announcementPipelineComposer = new Composer<string, string>(announcementActions);
      startAnnouncement = announcementPipelineComposer.CreatePipeline(announcement);
    }

    /*----------------Basic business operations---------------------------------------*/

    // Email creation operation
    public static string createEmail(string name)  {
      WriteLine($"\nEmail Created: {name}@lexmark.com");
      WriteLine("--------------------------------------------");
      return $"{name}@lexmark.com";
    }

    // Announcement generator operation
    public static string announcement(string message)  {
      WriteLine("\nAn important announcement has been sent to all departments!");
      WriteLine("--------------------------------------------------------------------");
      return $"{message}!";
    }

    // Email deletion operation
    public static string deleteEmail(string email)  {
      WriteLine($"\nEmail Deleted: {email}");
      WriteLine("--------------------------------------------------------------------");
      return $"Email {email} deleted.";
    }
  }
}