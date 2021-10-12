using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LAB.Catalogo.Services
{
    public interface IServiceBus<T> where T : class
    {
        void Publish(T message);
    }

    public class ServiceBus<T> : IServiceBus<T> where T : class
    {
        private readonly RabbitConfig _config;
        private readonly IModel _channel;
        private IConnection _connection;

        public ServiceBus(IOptions<RabbitConfig> config)
        {
            _config = config.Value;
            _channel = CreateChannel();
            DeclareAndBindQueueWithExchange();
        }

        private void DeclareAndBindQueueWithExchange()
        {
            _channel.ExchangeDeclare(_config.ExchangeName, _config.ExchangeType, durable: true, autoDelete: false, arguments: null);
            _channel.QueueDeclare(_config.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _config.QueueName, exchange: _config.ExchangeName, routingKey: _config.Routingkey);
        }

        public void Publish(T message)
        {
            Task.Run(() =>
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(
                    exchange: _config.ExchangeName,
                    routingKey: _config.Routingkey,
                    body: body
                );
            });
        }

        private IModel CreateChannel()
        {
            var factory = new ConnectionFactory();
            if(!string.IsNullOrEmpty(_config.ConnectionString)){
                factory.Uri = new Uri(_config.ConnectionString);
                System.Console.WriteLine(_config.ConnectionString);
            }else{
                factory.HostName = _config.HostName;
                factory.UserName = _config.UserName;
                factory.Password = _config.UserPassword;
                factory.VirtualHost = _config.VirtualHost;
                factory.Port = _config.Port;
            }
            _connection = factory.CreateConnection();
            return _connection.CreateModel();
        }
    }

    public class RabbitConfig
    {
        public string ConnectionString { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string Routingkey { get; set; }
    }

    public static class ServiceConfig
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitConfig>(config.GetSection(nameof(RabbitConfig)));
            services.AddScoped(typeof(IServiceBus<>), typeof(ServiceBus<>));
            return services;
        }
    }
}