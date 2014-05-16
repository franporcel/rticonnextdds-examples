using System;
using System.Collections.Generic;
using System.Text;
/* cfc_publisher.cs

   A publication of data of type cfc

   This file is derived from code automatically generated by the rtiddsgen 
   command:

   rtiddsgen -language C# -example <arch> cfc.idl

   Example publication of type cfc automatically generated by 
   'rtiddsgen'. To test them follow these steps:

   (1) Compile this file and the example subscription.

   (2) Start the subscription with the command
       objs\<arch>\cfc_subscriber <domain_id> <sample_count>
                
   (3) Start the publication with the command
       objs\<arch>\cfc_publisher <domain_id> <sample_count>

   (4) [Optional] Specify the list of discovery initial peers and 
       multicast receive addresses via an environment variable or a file 
       (in the current working directory) called NDDS_DISCOVERY_PEERS. 

   You can run any number of publishers and subscribers programs, and can 
   add and remove them dynamically from the domain.


   Example:

       To run the example application on domain <domain_id>:

       bin\<Debug|Release>\cfc_publisher <domain_id> <sample_count>
       bin\<Debug|Release>\cfc_subscriber <domain_id> <sample_count>

       
modification history
------------ -------       
*/

public class cfcPublisher {

    private static String initialize_array_of_strings (
        String str, Char value, int elements) {
            str = "";
            for (int i = 0; i < elements; ++i) {
                str += value;
            }
            return str.ToString();
    }

    public static void Main(string[] args) {

        // --- Get domain ID --- //
        int domain_id = 0;
        if (args.Length >= 1) {
            domain_id = Int32.Parse(args[0]);
        }

        // --- Get max loop count; 0 means infinite loop  --- //
        int sample_count = 0;
        if (args.Length >= 2) {
            sample_count = Int32.Parse(args[1]);
        }

        /* Uncomment this to turn on additional logging
        NDDS.ConfigLogger.get_instance().set_verbosity_by_category(
            NDDS.LogCategory.NDDS_CONFIG_LOG_CATEGORY_API, 
            NDDS.LogVerbosity.NDDS_CONFIG_LOG_VERBOSITY_STATUS_ALL);
        */
    
        // --- Run --- //
        try {
            cfcPublisher.publish(
                domain_id, sample_count);
        }
        catch(DDS.Exception)
        {
            Console.WriteLine("error in publisher");
        }
    }

    static void publish(int domain_id, int sample_count) {

        // --- Create participant --- //

        /* To customize participant QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.DomainParticipant participant =
            DDS.DomainParticipantFactory.get_instance().create_participant(
                domain_id,
                DDS.DomainParticipantFactory.PARTICIPANT_QOS_DEFAULT,
                null /* listener */,
                DDS.StatusMask.STATUS_MASK_NONE);
        if (participant == null) {
            shutdown(participant);
            throw new ApplicationException("create_participant error");
        }

        /* Start changes for custom_flowcontroller */

        /* If you want to change the Participant's QoS programmatically rather 
         * than using the XML file, you will need to add the following lines to 
         * your code and comment out the create_participant call above.
         */
        /* Get default participant QoS to customize */
/*        DDS.DomainParticipantQos participant_qos = 
                new DDS.DomainParticipantQos();

        try {
            DDS.DomainParticipantFactory.get_instance().
                get_default_participant_qos(participant_qos);
        } catch (DDS.Exception e) {
            Console.WriteLine("get_default_participant_qos error {0}", e);
            throw e;
        }

        // By default, data will be sent via shared memory _and_ UDPv4.  Because
        // the flowcontroller limits writes across all interfaces, this halves 
        // the effective send rate.  To avoid this, we enable only the UDPv4 
        // transport
         
        participant_qos.transport_builtin.mask = 
                (int) DDS.TransportBuiltinKind.TRANSPORTBUILTIN_UDPv4;
       
        // To create participant with default QoS, 
        // use DDS_PARTICIPANT_QOS_DEFAULT instead of participant_qos 
        DDS.DomainParticipant participant =
            DDS.DomainParticipantFactory.get_instance().create_participant(
                domain_id,
                participant_qos,
                null,
                DDS.StatusMask.STATUS_MASK_NONE);
        if (participant == null) {
            shutdown(participant);
            throw new ApplicationException("create_participant error");
        }
  */          
        /* End changes for custom_flow_controller */
     
        // --- Create publisher --- //

        /* To customize publisher QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Publisher publisher = participant.create_publisher(
        DDS.DomainParticipant.PUBLISHER_QOS_DEFAULT,
        null /* listener */,
        DDS.StatusMask.STATUS_MASK_NONE);
        if (publisher == null) {
            shutdown(participant);
            throw new ApplicationException("create_publisher error");
        }

        // --- Create topic --- //

        /* Register type before creating topic */
        System.String type_name = cfcTypeSupport.get_type_name();
        try {
            cfcTypeSupport.register_type(
                participant, type_name);
        }
        catch(DDS.Exception e) {
            Console.WriteLine("register_type error {0}", e);
            shutdown(participant);
            throw e;
        }

        /* To customize topic QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Topic topic = participant.create_topic(
            "Example cfc",
            type_name,
            DDS.DomainParticipant.TOPIC_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (topic == null) {
            shutdown(participant);
            throw new ApplicationException("create_topic error");
        }

        // --- Create writer --- //

        /* To customize data writer QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.DataWriter writer = publisher.create_datawriter(
            topic,
            DDS.Publisher.DATAWRITER_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (writer == null) {
            shutdown(participant);
            throw new ApplicationException("create_datawriter error");
        }

        /* Start changes for custom_flowcontroller */

        /* If you want to change the Datawriter's QoS programmatically rather 
         * than using the XML file, you will need to add the following lines to 
         * your code and comment out the create_datawriter call above.
         *
         * In this case we create the flowcontroller and the neccesary QoS
         * for the datawriter.
         */
/*
        DDS.FlowControllerProperty_t custom_fcp =
            new DDS.FlowControllerProperty_t();
        try {
            participant.get_default_flowcontroller_property(custom_fcp);
        } catch (DDS.Exception e) {
            Console.WriteLine("get_default_flowcontroller_property error {0}",
                    e);
            shutdown(participant);
            throw e;
        }

        // Don't allow too many tokens to accumulate
        custom_fcp.token_bucket.max_tokens =
            custom_fcp.token_bucket.tokens_added_per_period = 2;

        custom_fcp.token_bucket.tokens_leaked_per_period = 
            DDS.ResourceLimitsQosPolicy.LENGTH_UNLIMITED;

        //100 ms
        custom_fcp.token_bucket.period.sec = 0;
        custom_fcp.token_bucket.period.nanosec = 100000000;

        // The sample size is 1000, but the minimum bytes_per_token is 1024.
        // Furthermore, we want to allow some overhead.
        custom_fcp.token_bucket.bytes_per_token = 1024;

        // So, in summary, each token can be used to send about one message,
        // and we get 2 tokens every 100ms, so this limits transmissions to
        // about 20 messages per second.
        
        // Create flowcontroller and set properties
        String cfc_name = "custom_flowcontroller";
        DDS.FlowController cfc = participant.create_flowcontroller(
            cfc_name, custom_fcp);
        if (cfc == null) {
            shutdown(participant);
            throw new ApplicationException("create_flowcontroller error");
        }

        //Get default datawriter QoS to customize
        DDS.DataWriterQos datawriter_qos = new DDS.DataWriterQos();

        try {
            publisher.get_default_datawriter_qos(datawriter_qos);
        } catch (DDS.Exception e) {
            Console.WriteLine("get_default_datawriter_qos error {0}", e);
            shutdown(participant);
            throw e;
        }

        // As an alternative to increasing h istory depth, we can just
        // set the qos to keep all samples
        datawriter_qos.history.kind = 
                DDS.HistoryQosPolicyKind.KEEP_ALL_HISTORY_QOS;

        // Set flowcontroller for datawriter
        datawriter_qos.publish_mode.kind = 
            DDS.PublishModeQosPolicyKind.ASYNCHRONOUS_PUBLISH_MODE_QOS;
        datawriter_qos.publish_mode.flow_controller_name = cfc_name;

        // To create a datawriter with default QoS, use 
        // DDS.Publisher.DATAWRITER_QOS_DEFAULT instead of datawriter_qos
        DDS.DataWriter writer = publisher.create_datawriter(
           topic,
           datawriter_qos,
           null,
           DDS.StatusMask.STATUS_MASK_NONE);
        if (writer == null) {
            shutdown(participant);
            throw new ApplicationException("create_datawriter error");
        }

*/
        // End changes for custom_flowcontroller

        cfcDataWriter cfc_writer =
            (cfcDataWriter)writer;

        // --- Write --- //

        /* Create data sample for writing */
        cfc instance = cfcTypeSupport.create_data();
        if (instance == null) {
            shutdown(participant);
            throw new ApplicationException(
                "cfcTypeSupport.create_data error");
        }

        /* For a data type that has a key, if the same instance is going to be
           written multiple times, initialize the key here
           and register the keyed instance prior to writing */
        DDS.InstanceHandle_t instance_handle = DDS.InstanceHandle_t.HANDLE_NIL;
        /*
        instance_handle = cfc_writer.register_instance(instance);
        */

        /* Main loop */
        System.Int32 send_period = 1000; // milliseconds
        for (int count=0;
             (sample_count == 0) || (count < sample_count); ++count) {
            
            // Changes for custom_flowcontroller
            // Simulate bursty writer
            System.Threading.Thread.Sleep(send_period);
            for (int i = 0; i < 10; ++i) {
                int sample = count * 10 + i;
                Console.WriteLine("Writing cfc, sample {0}", sample);
                instance.x = sample;
                instance.str = 
                    initialize_array_of_strings(instance.str, '1', 999);
                instance.str += '0';
                try {
                    cfc_writer.write(instance, ref instance_handle);
                } catch (DDS.Exception e) {
                    Console.WriteLine("write error {0}", e);
                }
            }
        }
        //Console.WriteLine("{0}", instance.str);
        send_period = 4000;
        System.Threading.Thread.Sleep(send_period);

        /*
        try {
            cfc_writer.unregister_instance(
                instance, ref instance_handle);
        } catch(DDS.Exception e) {
            Console.WriteLine("unregister instance error: {0}", e);
        }
        */

        // --- Shutdown --- //

        /* Delete data sample */
        try {
            cfcTypeSupport.delete_data(instance);
        } catch(DDS.Exception e) {
            Console.WriteLine(
                "cfcTypeSupport.delete_data error: {0}", e);
        }

        /* Delete all entities */
        shutdown(participant);
    }

    static void shutdown(
        DDS.DomainParticipant participant) {

        /* Delete all entities */

        if (participant != null) {
            participant.delete_contained_entities();
            DDS.DomainParticipantFactory.get_instance().delete_participant(
                ref participant);
        }

        /* RTI Connext provides finalize_instance() method on
           domain participant factory for people who want to release memory
           used by the participant factory. Uncomment the following block of
           code for clean destruction of the singleton. */
        /*
        try {
            DDS.DomainParticipantFactory.finalize_instance();
        } catch (DDS.Exception e) {
            Console.WriteLine("finalize_instance error: {0}", e);
            throw e;
        }
        */
    }
}

