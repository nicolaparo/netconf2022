using LiteDB;
using NicolaParo.NetConf2022.NotificationSender.Models;
using System.Collections;
using System.Linq.Expressions;
using System.Text.Json;

namespace NicolaParo.NetConf2022.NotificationSender.Services.Repositories
{
    public interface IContactsRepository
    {
        Task<Guid> CreateContactAsync(ContactInfo contactData);
        Task<bool> DeleteContactAsync(Guid id);
        Task<Contact> GetContactByIdAsync(Guid id);
        Task<Contact[]> GetContactsAsync();
        Task<Contact[]> GetContactsAsync(Expression<Func<Contact, bool>> predicate);
        Task<bool> UpdateContactAsync(Guid id, ContactInfo contactData);

        event Func<Contact, Task> OnContactCreated;
        event Func<Contact, Task> OnContactUpdated;
        event Func<Contact, Task> OnContactDeleted;
    }

    public class ContactsRepository : IContactsRepository
    {
        private readonly NotificationSenderContext context;

        public ContactsRepository(NotificationSenderContext context)
        {
            this.context = context;
        }

        public Task<Contact[]> GetContactsAsync(Expression<Func<Contact, bool>> predicate)
        {
            var contacts = context.Contacts.Query().Where(predicate).ToArray();

            return Task.FromResult(contacts);
        }
        public Task<Contact[]> GetContactsAsync()
        {
            var contacts = context.Contacts.FindAll();

            return Task.FromResult(contacts.ToArray());
        }
        public Task<Contact> GetContactByIdAsync(Guid id)
        {
            var contact = context.Contacts.FindOne(c => c.Id == id);

            return Task.FromResult(contact);
        }
        public async Task<Guid> CreateContactAsync(ContactInfo contactData)
        {
            var contact = new Contact(contactData);

            context.Contacts.Insert(contact);

            if (OnContactCreated is not null)
                await OnContactCreated?.Invoke(contact);

            return contact.Id;
        }
        public async Task<bool> UpdateContactAsync(Guid id, ContactInfo contactData)
        {
            var contact = context.Contacts.FindOne(c => c.Id == id);
            if (contact is null)
                return false;

            contact = new Contact(contactData) with { Id = contact.Id, UpdatedAt = DateTime.UtcNow };

            context.Contacts.Update(contact);

            if (OnContactUpdated is not null)
                await OnContactUpdated(contact);

            return true;
        }
        public async Task<bool> DeleteContactAsync(Guid id)
        {
            var contact = context.Contacts.FindOne(c => c.Id == id);
            if (contact is null)
                return false;

            context.Contacts.Delete(id);

            if (OnContactDeleted is not null)
                await OnContactDeleted(contact);

            return true;
        }

        public event Func<Contact, Task> OnContactCreated;
        public event Func<Contact, Task> OnContactUpdated;
        public event Func<Contact, Task> OnContactDeleted;

    }
}